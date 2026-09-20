using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 读写excel
/// 读取 EditorTools/Excel 下的xlsx文件，生成ScriptableObject类代码(.cs)和配置数据(.asset)
/// excel格式：
/// 1行 A列=类名 B列=说明
/// 2行 A列填 class 或 list 决定结构（不填默认list）：
///   list=每行一条数据，存到类的list字段里
///   class=每列是一个参数，数据沿列往下排，参数直接放在类里
/// 3行=每列的注释
/// 4行=字段名，字段名后面加(类型)指定类型，不填就自动判断 int/float/string/List/Dictionary
/// 5行起=数据，一个excel生成一个.asset，名字跟excel一样
/// excel里空的引用类型（GameObject/Sprite）不会覆盖asset里已有的值
/// 可以把asset里非基础类型的值（GameObject/Sprite/Vector3等）写回excel
/// </summary>
public class ReadWriteExcel : EditorWindow
{
    //excel目录
    string excelPath = "Assets/EditorTools/Excel";
    //生成的类代码目录
    string classPath = "Assets/Scripts/Excel";
    //生成的asset目录
    string assetPath = "Assets/Resources/Excel";

    public const string SessionFilesKey = "rwExcel.files";
    public const string SessionAssetDirKey = "rwExcel.assetDir";

    //利用构造函数来设置窗口名称
    ReadWriteExcel()
    {
        this.titleContent = new GUIContent("读写excel");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/读写excel")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ReadWriteExcel));
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("excel格式：\n1行 A列=类名 B列=说明\n2行 A列=class或list：class=每列一个参数往下排，list=每行一条数据（不填默认list）\n3行=注释\n4行=字段名(类型)，不填类型自动判断 int/float/string/List/Dictionary\n5行起=数据，一个excel生成一个.asset，名字跟excel一样\n读取时excel里空的引用类型不覆盖asset里已有的值\n写回excel把asset里GameObject/Sprite/Vector3等非基础类型的值写回excel", MessageType.Info);
        excelPath = EditorGUILayout.TextField("excel路径", excelPath);
        classPath = EditorGUILayout.TextField("类代码路径", classPath);
        assetPath = EditorGUILayout.TextField("asset输出路径", assetPath);
        if (GUILayout.Button("读取生成"))
        {
            Generate();
        }
        if (GUILayout.Button("写回excel"))
        {
            WriteBackToExcel();
        }
    }

    /// <summary>
    /// 读取excel目录下的所有xlsx，生成类代码和asset
    /// </summary>
    void Generate()
    {
        if (!Directory.Exists(ToAbsolute(excelPath)))
        {
            Debug.LogError("excel目录不存在：" + excelPath);
            return;
        }
        List<string> files = new List<string>();
        foreach (string f in Directory.GetFiles(ToAbsolute(excelPath), "*.xlsx"))
        {
            if (Path.GetFileName(f).StartsWith("~$")) continue; //excel打开时的临时文件
            files.Add(f);
        }
        if (files.Count == 0)
        {
            Debug.LogError("excel目录里没有xlsx文件：" + excelPath);
            return;
        }
        bool needCompile = false;
        foreach (string file in files)
        {
            ExcelData data = ParseXlsx(file);
            if (data == null) continue;
            List<ExcelField> fields = ResolveFields(data);
            if (fields == null) continue;
            string code = BuildClassCode(data, fields, file, assetPath);
            string csPath = classPath + "/" + data.className + ".cs";
            if (File.Exists(ToAbsolute(csPath)))
            {
                string old = File.ReadAllText(ToAbsolute(csPath));
                if (old == code) continue; //代码没变不用重新编译
                //不是本工具生成的类不覆盖
                if (old.Contains("自动生成") == false)
                {
                    Debug.LogWarning(csPath + " 是手动写的类，不覆盖，直接用它生成asset");
                    continue;
                }
            }
            else
            {
                //类已经在别处存在就不再生成新的
                if (FindType(data.className) != null)
                {
                    Debug.LogWarning("已存在类 " + data.className + "，不重新生成代码");
                    continue;
                }
            }
            WriteFile(csPath, code);
            needCompile = true;
            Debug.Log("生成类代码：" + csPath);
        }
        if (needCompile)
        {
            //类代码改过，等编译完再生成asset
            SessionState.SetString(SessionFilesKey, string.Join("\n", files.ToArray()));
            SessionState.SetString(SessionAssetDirKey, assetPath);
            AssetDatabase.Refresh();
            Debug.Log("等待编译完成，完成后自动生成asset...");
        }
        else
        {
            CreateAllAssets(assetPath, files.ToArray());
        }
    }

    /// <summary>
    /// 用excel数据生成asset，一个excel一个.asset，名字跟excel一样，数据存到类的list字段里
    /// </summary>
    public static void CreateAllAssets(string assetDir, string[] files)
    {
        if (assetDir == "") assetDir = "Assets/Resources/Excel";
        int total = 0;
        foreach (string file in files)
        {
            if (file == "" || Path.GetFileName(file).StartsWith("~$")) continue;
            ExcelData data = ParseXlsx(file);
            if (data == null) continue;
            List<ExcelField> fields = ResolveFields(data);
            if (fields == null) continue;
            Type type = FindType(data.className);
            if (type == null)
            {
                Debug.LogError("找不到类 " + data.className + "，请检查类代码是否编译成功：" + file);
                continue;
            }
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                Debug.LogError(data.className + " 不是ScriptableObject：" + file);
                continue;
            }
            //asset名字跟excel名字一样
            string assetName = SanitizeFileName(Path.GetFileNameWithoutExtension(file));
            if (assetName == "") assetName = data.className;
            string path = assetDir + "/" + assetName + ".asset";
            //class结构：参数直接放在类里
            if (data.structure == "class")
            {
                if (CreateOneAssetSingle(data, fields, type, path, assetDir)) total++;
                continue;
            }
            //建目录
            string absDir = ToAbsolute(assetDir);
            if (!Directory.Exists(absDir)) Directory.CreateDirectory(absDir);
            //数据放在类的list字段里
            FieldInfo listField = type.GetField("list", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (listField == null || !listField.FieldType.IsGenericType || listField.FieldType.GetGenericArguments().Length != 1)
            {
                Debug.LogError(data.className + " 没有 public List<> list 字段，请重新点读取生成：" + file);
                continue;
            }
            Type itemType = listField.FieldType.GetGenericArguments()[0];
            //已有asset就覆盖，类型变了就删了重建
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            bool isNew = false;
            if (so != null && so.GetType() != type)
            {
                AssetDatabase.DeleteAsset(path);
                so = null;
            }
            if (so == null)
            {
                so = ScriptableObject.CreateInstance(type);
                isNew = true;
            }
            //原来asset里的数据按key存起来，excel里空的引用类型不覆盖，用回原来的值
            Dictionary<string, object> oldByKey = new Dictionary<string, object>();
            IList oldList = null;
            if (!isNew)
            {
                oldList = (IList)listField.GetValue(so);
                if (oldList != null)
                {
                    FieldInfo keyField = itemType.GetField(fields[0].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (object oi in oldList)
                    {
                        if (keyField == null) break;
                        string k = ValueToKeyString(keyField.GetValue(oi));
                        if (k != "" && !oldByKey.ContainsKey(k)) oldByKey[k] = oi;
                    }
                }
            }
            IList list = (IList)Activator.CreateInstance(listField.FieldType);
            bool ok = true;
            for (int r = 0; r < data.rows.Count; r++)
            {
                object item = Activator.CreateInstance(itemType);
                string keyStr = data.rows[r][0].Trim();
                for (int i = 0; i < fields.Count; i++)
                {
                    FieldInfo fi = itemType.GetField(fields[i].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (fi == null)
                    {
                        Debug.LogError(data.className + "Item 没有字段 " + fields[i].name + "，类代码可能没更新，请重新点读取生成");
                        ok = false;
                        break;
                    }
                    string cellStr = data.rows[r][i].Trim();
                    //excel里空的引用类型不覆盖，用回原来asset里对应key的值
                    if (cellStr == "" && IsRefType(fields[i].csType))
                    {
                        object oldItem = null;
                        if (oldByKey.TryGetValue(keyStr, out oldItem) == false && oldList != null && r < oldList.Count)
                        {
                            oldItem = oldList[r];
                        }
                        if (oldItem != null)
                        {
                            fi.SetValue(item, fi.GetValue(oldItem));
                            continue;
                        }
                    }
                    fi.SetValue(item, ParseValue(cellStr, fields[i].csType));
                }
                if (!ok) break;
                //字典同步到序列化列表
                ISerializationCallbackReceiver cb = item as ISerializationCallbackReceiver;
                if (cb != null) cb.OnBeforeSerialize();
                list.Add(item);
            }
            if (!ok)
            {
                if (isNew) UnityEngine.Object.DestroyImmediate(so);
                continue;
            }
            listField.SetValue(so, list);
            if (isNew) AssetDatabase.CreateAsset(so, path);
            EditorUtility.SetDirty(so);
            Debug.Log("生成配置：" + path + "（" + list.Count + "条数据）");
            total++;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("读写excel完成，共生成 " + total + " 个配置");
    }

    /// <summary>
    /// class结构：生成一个asset，每列是一个参数，数据沿列往下读
    /// </summary>
    static bool CreateOneAssetSingle(ExcelData data, List<ExcelField> fields, Type type, string path, string assetDir)
    {
        //建目录
        string absDir = ToAbsolute(assetDir);
        if (!Directory.Exists(absDir)) Directory.CreateDirectory(absDir);
        //已有asset就覆盖，类型变了就删了重建
        ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
        bool isNew = false;
        if (so != null && so.GetType() != type)
        {
            AssetDatabase.DeleteAsset(path);
            so = null;
        }
        if (so == null)
        {
            so = ScriptableObject.CreateInstance(type);
            isNew = true;
        }
        bool ok = true;
        for (int i = 0; i < fields.Count; i++)
        {
            FieldInfo fi = type.GetField(fields[i].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fi == null)
            {
                Debug.LogError(data.className + " 没有字段 " + fields[i].name + "，类代码可能没更新，请重新点读取生成");
                ok = false;
                break;
            }
            //每列往下读非空格子
            List<string> cells = new List<string>();
            foreach (string[] row in data.rows)
            {
                string v = row[i].Trim();
                if (v != "") cells.Add(v);
            }
            //excel里空的引用类型不覆盖，用回原来asset里的值
            if (cells.Count == 0 && IsRefType(fields[i].csType) && !isNew)
            {
                continue;
            }
            fi.SetValue(so, ParseColumnValue(cells, fields[i].csType));
        }
        if (!ok)
        {
            if (isNew) UnityEngine.Object.DestroyImmediate(so);
            return false;
        }
        //字典同步到序列化列表
        ISerializationCallbackReceiver cb = so as ISerializationCallbackReceiver;
        if (cb != null) cb.OnBeforeSerialize();
        if (isNew) AssetDatabase.CreateAsset(so, path);
        EditorUtility.SetDirty(so);
        Debug.Log("生成配置：" + path + "（单条数据）");
        return true;
    }

    #region 解析excel

    /// <summary>
    /// 解析xlsx，读取第一个sheet
    /// </summary>
    static ExcelData ParseXlsx(string file)
    {
        //excel打开时的缓存文件，读写都忽略
        if (Path.GetFileName(file).StartsWith("~$")) return null;
        try
        {
            using (FileStream fs = OpenXlsx(file, FileAccess.Read))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                List<string> sst = ReadSharedStrings(zip);
                string sheetXml = ReadFirstSheet(zip);
                if (sheetXml == null)
                {
                    Debug.LogError("excel里没有sheet：" + file);
                    return null;
                }
                Dictionary<long, string> cells = ReadCells(sheetXml, sst);
                return BuildData(cells, file);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("解析excel失败：" + file + " " + e.Message);
            return null;
        }
    }

    static List<string> ReadSharedStrings(ZipArchive zip)
    {
        List<string> list = new List<string>();
        ZipArchiveEntry entry = zip.GetEntry("xl/sharedStrings.xml");
        if (entry == null) return list;
        XmlDocument doc = LoadXml(entry);
        foreach (XmlNode si in doc.SelectNodes("//*[local-name()='si']"))
        {
            string text = "";
            foreach (XmlNode t in si.SelectNodes(".//*[local-name()='t']")) text += t.InnerText;
            list.Add(text);
        }
        return list;
    }

    /// <summary>
    /// 读第一个sheet的内容，workbook.xml里找第一个sheet再通过rels找文件
    /// </summary>
    static string ReadFirstSheet(ZipArchive zip)
    {
        ZipArchiveEntry entry = FindSheetEntry(zip);
        if (entry == null) return null;
        using (StreamReader sr = new StreamReader(entry.Open(), Encoding.UTF8)) return sr.ReadToEnd();
    }

    /// <summary>
    /// 找第一个sheet的zip条目
    /// </summary>
    static ZipArchiveEntry FindSheetEntry(ZipArchive zip)
    {
        ZipArchiveEntry wb = zip.GetEntry("xl/workbook.xml");
        if (wb != null)
        {
            XmlDocument doc = LoadXml(wb);
            XmlNode first = doc.SelectSingleNode("//*[local-name()='sheet']");
            if (first != null)
            {
                XmlAttribute ra = first.Attributes["r:id"];
                string rid = ra == null ? null : ra.Value;
                if (rid != null)
                {
                    ZipArchiveEntry rels = zip.GetEntry("xl/_rels/workbook.xml.rels");
                    if (rels != null)
                    {
                        XmlDocument rdoc = LoadXml(rels);
                        foreach (XmlNode rel in rdoc.SelectNodes("//*[local-name()='Relationship']"))
                        {
                            XmlAttribute ia = rel.Attributes["Id"];
                            if (ia != null && ia.Value == rid)
                            {
                                XmlAttribute ta = rel.Attributes["Target"];
                                if (ta != null)
                                {
                                    string target = ta.Value.StartsWith("/") ? ta.Value.Substring(1) : "xl/" + ta.Value;
                                    ZipArchiveEntry se = zip.GetEntry(target);
                                    if (se != null) return se;
                                }
                            }
                        }
                    }
                }
            }
        }
        //回退：找sheet编号最小的
        ZipArchiveEntry best = null;
        int bestNum = int.MaxValue;
        foreach (ZipArchiveEntry e in zip.Entries)
        {
            Match m = Regex.Match(e.FullName, @"xl/worksheets/sheet(\d+)\.xml$");
            if (m.Success)
            {
                int num = int.Parse(m.Groups[1].Value);
                if (num < bestNum)
                {
                    bestNum = num;
                    best = e;
                }
            }
        }
        return best;
    }

    static Dictionary<long, string> ReadCells(string sheetXml, List<string> sst)
    {
        Dictionary<long, string> cells = new Dictionary<long, string>();
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(sheetXml);
        foreach (XmlNode c in doc.SelectNodes("//*[local-name()='c']"))
        {
            XmlAttribute ra = c.Attributes["r"];
            if (ra == null) continue;
            string r = ra.Value;
            int row, col;
            ParseCellRef(r, out row, out col);
            XmlAttribute ta = c.Attributes["t"];
            string t = ta == null ? "" : ta.Value;
            string v = "";
            if (t == "inlineStr")
            {
                foreach (XmlNode tt in c.SelectNodes(".//*[local-name()='t']")) v += tt.InnerText;
            }
            else
            {
                XmlNode vn = c.SelectSingleNode("*[local-name()='v']");
                if (vn != null)
                {
                    v = vn.InnerText;
                    if (t == "s")
                    {
                        int idx;
                        v = int.TryParse(v, out idx) && idx >= 0 && idx < sst.Count ? sst[idx] : "";
                    }
                    else if (t == "b")
                    {
                        v = v == "1" ? "true" : "false";
                    }
                }
            }
            cells[row * 16384L + col] = v;
        }
        return cells;
    }

    static void ParseCellRef(string r, out int row, out int col)
    {
        Match m = Regex.Match(r, @"([A-Z]+)(\d+)");
        string letters = m.Groups[1].Value;
        col = 0;
        foreach (char ch in letters) col = col * 26 + (ch - 'A' + 1);
        col -= 1;
        row = int.Parse(m.Groups[2].Value);
    }

    static string GetCell(Dictionary<long, string> cells, int row, int col)
    {
        string v;
        return cells.TryGetValue(row * 16384L + col, out v) ? v : "";
    }

    static ExcelData BuildData(Dictionary<long, string> cells, string file)
    {
        ExcelData data = new ExcelData();
        data.className = GetCell(cells, 1, 0).Trim();
        data.desc = GetCell(cells, 1, 1).Trim();
        //2行A列填class还是list决定结构，不填默认list
        string structure = GetCell(cells, 2, 0).Trim().ToLower();
        if (structure == "class") data.structure = "class";
        else if (structure != "" && structure != "list") Debug.LogWarning(file + "：2行A列只能填 class 或 list，按 list 处理");
        if (data.className == "")
        {
            Debug.LogError(file + "：1行A列必须填ScriptableObject类名");
            return null;
        }
        if (!Regex.IsMatch(data.className, @"^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            Debug.LogError(file + "：类名不合法：" + data.className);
            return null;
        }
        //3行注释 4行字段名，列数按4行字段名算
        int lastField = -1;
        for (int col = 0; col < 1000; col++)
        {
            if (GetCell(cells, 4, col) != "") lastField = col;
        }
        if (lastField < 0)
        {
            Debug.LogError(file + "：4行没有字段名");
            return null;
        }
        for (int col = 0; col <= lastField; col++)
        {
            data.comments.Add(GetCell(cells, 3, col).Trim());
            data.fieldDefs.Add(GetCell(cells, 4, col).Trim());
        }
        //5行起数据，整行都空就结束
        for (int row = 5; ; row++)
        {
            string[] rowCells = new string[data.fieldDefs.Count];
            bool isEmpty = true;
            for (int col = 0; col < rowCells.Length; col++)
            {
                rowCells[col] = GetCell(cells, row, col);
                if (rowCells[col] != "") isEmpty = false;
            }
            if (isEmpty) break;
            data.rows.Add(rowCells);
        }
        if (data.rows.Count == 0)
        {
            Debug.LogError(file + "：5行起没有数据");
            return null;
        }
        return data;
    }

    static XmlDocument LoadXml(ZipArchiveEntry entry)
    {
        using (StreamReader sr = new StreamReader(entry.Open(), Encoding.UTF8))
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            return doc;
        }
    }

    /// <summary>
    /// 打开excel文件。excel开着的时候会锁文件（自动保存会短暂锁），用最宽松的共享方式多试几次
    /// </summary>
    static FileStream OpenXlsx(string file, FileAccess access)
    {
        Exception lastError = null;
        for (int i = 0; i < 5; i++)
        {
            try
            {
                return File.Open(file, FileMode.Open, access, FileShare.ReadWrite | FileShare.Delete);
            }
            catch (Exception e)
            {
                lastError = e;
                System.Threading.Thread.Sleep(100 * (i + 1));
            }
        }
        throw new IOException("文件被其他程序占用，请关闭excel里的这个文件再试：" + file, lastError);
    }

    #endregion

    #region 字段和类型

    class ExcelData
    {
        public string className = "";
        public string desc = "";
        //结构：class=每列一个参数往下排，list=每行一条数据
        public string structure = "list";
        public List<string> comments = new List<string>();
        public List<string> fieldDefs = new List<string>();
        public List<string[]> rows = new List<string[]>();
    }

    class ExcelField
    {
        public string name = "";
        public string explicitType = null;
        public string csType = "";
    }

    /// <summary>
    /// 解析字段名和类型，字段名后面(类型)指定类型，没填就按数据自动判断
    /// </summary>
    static List<ExcelField> ResolveFields(ExcelData data)
    {
        List<ExcelField> fields = new List<ExcelField>();
        HashSet<string> names = new HashSet<string>();
        for (int i = 0; i < data.fieldDefs.Count; i++)
        {
            string def = data.fieldDefs[i];
            Match m = Regex.Match(def, @"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*(?:\(\s*(.+?)\s*\))?\s*$");
            if (!m.Success)
            {
                Debug.LogError("字段名格式错误(4行第" + (i + 1) + "列)：" + def);
                return null;
            }
            ExcelField f = new ExcelField();
            f.name = m.Groups[1].Value;
            f.explicitType = m.Groups[2].Success ? m.Groups[2].Value : null;
            if (!names.Add(f.name))
            {
                Debug.LogError("字段名重复：" + f.name);
                return null;
            }
            fields.Add(f);
        }
        for (int i = 0; i < fields.Count; i++)
        {
            ExcelField f = fields[i];
            if (f.explicitType != null)
            {
                string t = NormalizeType(f.explicitType);
                if (t == null)
                {
                    Debug.LogError("不支持的类型：" + f.explicitType + "（字段：" + f.name + "）");
                    return null;
                }
                //class结构：基础类型一列有多个值就按List<类型>处理
                if (data.structure == "class" && !IsListType(t) && !IsDictType(t))
                {
                    int count = 0;
                    foreach (string[] row in data.rows)
                    {
                        if (row[i].Trim() != "") count++;
                    }
                    if (count > 1)
                    {
                        t = "List<" + t + ">";
                        Debug.Log("字段 " + f.name + " 一列有 " + count + " 个值，按 " + t + " 处理");
                    }
                }
                f.csType = t;
            }
            else
            {
                f.csType = AutoDetectType(data, i);
            }
            Debug.Log("字段 " + f.name + " 类型 " + f.csType);
        }
        return fields;
    }

    static string[] scalarTypes = { "int", "float", "string", "bool", "GameObject", "Sprite", "Vector2", "Vector3" };

    static string NormalizeScalar(string t)
    {
        foreach (string s in scalarTypes)
        {
            if (string.Equals(s, t, StringComparison.OrdinalIgnoreCase)) return s;
        }
        return null;
    }

    static string NormalizeType(string t)
    {
        t = t.Trim();
        string s = NormalizeScalar(t);
        if (s != null) return s;
        //支持嵌套：List<List<int>>、Dictionary<int,List<int>>等
        if (t.StartsWith("List<") && t.EndsWith(">"))
        {
            string inner = NormalizeType(t.Substring(5, t.Length - 6));
            return inner == null ? null : "List<" + inner + ">";
        }
        if (t.StartsWith("Dictionary<") && t.EndsWith(">"))
        {
            //在顶层逗号拆key和value
            string inner = t.Substring(11, t.Length - 12);
            int comma = FindTopLevelComma(inner);
            if (comma < 0) return null;
            string k = NormalizeType(inner.Substring(0, comma));
            string v = NormalizeType(inner.Substring(comma + 1));
            if (k == null || v == null || k.Contains("<")) return null; //key只能是基础类型
            return "Dictionary<" + k + "," + v + ">";
        }
        return null;
    }

    /// <summary>
    /// 找<>深度为0的逗号位置
    /// </summary>
    static int FindTopLevelComma(string s)
    {
        int depth = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
            if (ch == '<') depth++;
            else if (ch == '>') depth--;
            else if (ch == ',' && depth == 0) return i;
        }
        return -1;
    }

    /// <summary>
    /// 按一列的数据自动判断类型：int -> float -> string，[..]是List，{..}是Dictionary
    /// </summary>
    static string AutoDetectType(ExcelData data, int col)
    {
        List<string> values = new List<string>();
        foreach (string[] row in data.rows)
        {
            string v = row[col].Trim();
            if (v != "") values.Add(v);
        }
        if (values.Count == 0) return "string";
        //class结构：一列有多个值就是List，每个格子是一个元素
        if (data.structure == "class" && values.Count > 1)
        {
            bool allCellList = true;
            foreach (string v in values)
            {
                if (!IsListStr(v)) allCellList = false;
            }
            if (allCellList) return "List<List<" + DetectListElemType(values) + ">>";
            return "List<" + DetectElemTypeFromCells(values) + ">";
        }
        bool allList = true;
        bool allDict = true;
        foreach (string v in values)
        {
            if (!IsListStr(v)) allList = false;
            if (!IsDictStr(v)) allDict = false;
        }
        if (allList) return "List<" + DetectListElemType(values) + ">";
        if (allDict)
        {
            string kt, vt;
            DetectDictTypes(values, out kt, out vt);
            return "Dictionary<" + kt + "," + vt + ">";
        }
        //标量：全int就是int，全数字就是float，别的都是string
        bool allInt = true;
        bool allFloat = true;
        foreach (string v in values)
        {
            if (!IsIntStr(v)) allInt = false;
            if (!IsFloatStr(v)) allFloat = false;
        }
        if (allInt) return "int";
        if (allFloat) return "float";
        if (!allList && values.Exists(IsListStr)) Debug.LogWarning("字段数据混合了list和普通值，按string处理：" + values[0]);
        else if (!allDict && values.Exists(IsDictStr)) Debug.LogWarning("字段数据混合了dict和普通值，按string处理：" + values[0]);
        return "string";
    }

    static string DetectListElemType(List<string> values)
    {
        List<string> elems = new List<string>();
        foreach (string v in values) elems.AddRange(SplitCsv(StripBrackets(v)));
        //元素本身都是list就是列表里带列表
        bool hasElem = false;
        bool allElemList = true;
        foreach (string e in elems)
        {
            string ee = Unquote(e);
            if (ee == "") continue;
            hasElem = true;
            if (!IsListStr(ee)) allElemList = false;
        }
        if (hasElem && allElemList) return "List<" + DetectListElemType(elems) + ">";
        return DetectElemTypeFromCells(elems);
    }

    /// <summary>
    /// 每个值整体当元素判断基础类型
    /// </summary>
    static string DetectElemTypeFromCells(List<string> values)
    {
        bool allInt = true;
        bool allFloat = true;
        foreach (string v in values)
        {
            string e = Unquote(v);
            if (e == "") continue;
            if (!IsIntStr(e)) allInt = false;
            if (!IsFloatStr(e)) allFloat = false;
        }
        if (allInt) return "int";
        if (allFloat) return "float";
        return "string";
    }

    static void DetectDictTypes(List<string> values, out string keyType, out string valueType)
    {
        List<string> keys = new List<string>();
        List<string> vals = new List<string>();
        foreach (string v in values)
        {
            List<string> parts = SplitCsv(StripBrackets(v));
            for (int i = 0; i < parts.Count; i++)
            {
                if (i % 2 == 0) keys.Add(Unquote(parts[i]));
                else vals.Add(parts[i]);
            }
        }
        bool allKeyInt = true;
        foreach (string k in keys) if (k != "" && !IsIntStr(k)) allKeyInt = false;
        keyType = allKeyInt ? "int" : "string";
        //值都是list就是字典里带列表
        bool hasVal = false;
        bool allValList = true;
        foreach (string v2 in vals)
        {
            if (v2 == "") continue;
            hasVal = true;
            if (!IsListStr(v2)) allValList = false;
        }
        if (hasVal && allValList)
        {
            valueType = "List<" + DetectListElemType(vals) + ">";
            return;
        }
        bool allValInt = true;
        bool allValFloat = true;
        foreach (string v2 in vals) if (v2 != "" && !IsIntStr(v2)) allValInt = false;
        foreach (string v2 in vals) if (v2 != "" && !IsFloatStr(v2)) allValFloat = false;
        valueType = allValInt ? "int" : (allValFloat ? "float" : "string");
    }

    static bool IsListStr(string s) { return s.StartsWith("[") && s.EndsWith("]"); }

    static bool IsDictStr(string s) { return s.StartsWith("{") && s.EndsWith("}"); }

    static bool IsIntStr(string s)
    {
        int iv;
        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out iv);
    }

    static bool IsFloatStr(string s)
    {
        float fv;
        return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out fv);
    }

    static string StripBrackets(string s)
    {
        s = s.Trim();
        if (s.Length >= 2 && ((s[0] == '[' && s[s.Length - 1] == ']') || (s[0] == '{' && s[s.Length - 1] == '}') || (s[0] == '(' && s[s.Length - 1] == ')')))
            return s.Substring(1, s.Length - 2);
        return s;
    }

    static string Unquote(string s)
    {
        if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"') return s.Substring(1, s.Length - 2);
        return s;
    }

    /// <summary>
    /// 按逗号分割，引号和[]{}()里面的逗号不分割，引号会被去掉
    /// </summary>
    static List<string> SplitCsv(string s)
    {
        List<string> list = new List<string>();
        StringBuilder cur = new StringBuilder();
        bool inQuote = false;
        int depth = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
            if (ch == '"')
            {
                inQuote = !inQuote;
                continue;
            }
            if (!inQuote)
            {
                if (ch == '[' || ch == '{' || ch == '(') depth++;
                else if (ch == ']' || ch == '}' || ch == ')') depth--;
                else if (ch == ',' && depth == 0)
                {
                    list.Add(cur.ToString().Trim());
                    cur.Length = 0;
                    continue;
                }
            }
            cur.Append(ch);
        }
        list.Add(cur.ToString().Trim());
        return list;
    }

    #endregion

    #region 生成类代码

    static bool IsListType(string t) { return t.StartsWith("List<"); }

    static bool IsDictType(string t) { return t.StartsWith("Dictionary<"); }

    static void GetDictInner(string t, out string k, out string v)
    {
        string inner = t.Substring(11, t.Length - 12);
        string[] parts = inner.Split(',');
        k = parts[0].Trim();
        v = parts[1].Trim();
    }

    static string BuildClassCode(ExcelData data, List<ExcelField> fields, string excelFile, string assetDir)
    {
        if (data.structure == "class")
        {
            return BuildClassCodeSingle(data, fields, excelFile, assetDir);
        }
        string itemName = data.className + "Item";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("//自动生成，请勿手动修改！来源：" + excelFile);
        sb.AppendLine("#pragma warning disable 0108"); //字段名可能跟Object.name等重名
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("");
        if (data.desc != "")
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// " + data.desc);
            sb.AppendLine("/// </summary>");
        }
        sb.AppendLine("public class " + data.className + " : ScriptableObject");
        sb.AppendLine("{");
        sb.AppendLine("    [Header(\"数据列表\")]");
        sb.AppendLine("    public List<" + itemName + "> list = new List<" + itemName + ">();");
        //asset在Resources下面的话生成self单例，跟AdidAsset一样用
        if (assetDir.StartsWith("Assets/Resources/"))
        {
            string resPath = assetDir.Substring("Assets/Resources/".Length).TrimEnd('/') + "/" + Path.GetFileNameWithoutExtension(excelFile);
            sb.AppendLine("");
            sb.AppendLine("    static " + data.className + " _self;");
            sb.AppendLine("    public static " + data.className + " self");
            sb.AppendLine("    {");
            sb.AppendLine("        get");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_self == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                _self = Resources.Load<" + data.className + ">(\"" + resPath + "\");");
            sb.AppendLine("            }");
            sb.AppendLine("            return _self;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
        }
        sb.AppendLine("}");
        sb.AppendLine("");
        bool hasDict = false;
        foreach (ExcelField f in fields)
        {
            if (IsDictType(f.csType)) hasDict = true;
        }
        sb.AppendLine("[Serializable]");
        sb.AppendLine("public class " + itemName + (hasDict ? " : ISerializationCallbackReceiver" : ""));
        sb.AppendLine("{");
        for (int i = 0; i < fields.Count; i++)
        {
            ExcelField f = fields[i];
            string comment = data.comments[i].Replace("\"", "'").Replace("\n", " ");
            if (comment != "") sb.AppendLine("    [Header(\"" + comment + "\")]");
            string init = "";
            if (IsListType(f.csType) || IsDictType(f.csType)) init = " = new " + f.csType + "()";
            sb.AppendLine("    public " + f.csType + " " + f.name + init + ";");
        }
        if (hasDict) AppendDictSerialize(sb, fields);
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Dictionary不能直接序列化，生成keys/values两个list和存读同步函数
    /// </summary>
    static void AppendDictSerialize(StringBuilder sb, List<ExcelField> fields)
    {
        sb.AppendLine("");
        foreach (ExcelField f in fields)
        {
            if (!IsDictType(f.csType)) continue;
            string kt, vt;
            GetDictInner(f.csType, out kt, out vt);
            sb.AppendLine("    [SerializeField, HideInInspector] private List<" + kt + "> " + f.name + "_keys = new List<" + kt + ">();");
            sb.AppendLine("    [SerializeField, HideInInspector] private List<" + vt + "> " + f.name + "_values = new List<" + vt + ">();");
        }
        sb.AppendLine("");
        sb.AppendLine("    public void OnBeforeSerialize()");
        sb.AppendLine("    {");
        foreach (ExcelField f in fields)
        {
            if (!IsDictType(f.csType)) continue;
            sb.AppendLine("        " + f.name + "_keys.Clear();");
            sb.AppendLine("        " + f.name + "_values.Clear();");
            sb.AppendLine("        foreach (var kv in " + f.name + ")");
            sb.AppendLine("        {");
            sb.AppendLine("            " + f.name + "_keys.Add(kv.Key);");
            sb.AppendLine("            " + f.name + "_values.Add(kv.Value);");
            sb.AppendLine("        }");
        }
        sb.AppendLine("    }");
        sb.AppendLine("");
        sb.AppendLine("    public void OnAfterDeserialize()");
        sb.AppendLine("    {");
        foreach (ExcelField f in fields)
        {
            if (!IsDictType(f.csType)) continue;
            sb.AppendLine("        " + f.name + " = new " + f.csType + "();");
            sb.AppendLine("        int count = Mathf.Min(" + f.name + "_keys.Count, " + f.name + "_values.Count);");
            sb.AppendLine("        for (int i = 0; i < count; i++)");
            sb.AppendLine("        {");
            sb.AppendLine("            " + f.name + "[" + f.name + "_keys[i]] = " + f.name + "_values[i];");
            sb.AppendLine("        }");
        }
        sb.AppendLine("    }");
    }

    /// <summary>
    /// class结构的类：每个列一个参数，参数直接放在类里
    /// </summary>
    static string BuildClassCodeSingle(ExcelData data, List<ExcelField> fields, string excelFile, string assetDir)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("//自动生成，请勿手动修改！来源：" + excelFile);
        sb.AppendLine("#pragma warning disable 0108"); //字段名可能跟Object.name等重名
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("");
        if (data.desc != "")
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// " + data.desc);
            sb.AppendLine("/// </summary>");
        }
        bool hasDict = false;
        foreach (ExcelField f in fields)
        {
            if (IsDictType(f.csType)) hasDict = true;
        }
        sb.AppendLine("public class " + data.className + " : ScriptableObject" + (hasDict ? ", ISerializationCallbackReceiver" : ""));
        sb.AppendLine("{");
        for (int i = 0; i < fields.Count; i++)
        {
            ExcelField f = fields[i];
            string comment = data.comments[i].Replace("\"", "'").Replace("\n", " ");
            if (comment != "") sb.AppendLine("    [Header(\"" + comment + "\")]");
            string init = "";
            if (IsListType(f.csType) || IsDictType(f.csType)) init = " = new " + f.csType + "()";
            sb.AppendLine("    public " + f.csType + " " + f.name + init + ";");
        }
        if (hasDict) AppendDictSerialize(sb, fields);
        //asset在Resources下面的话生成self单例，跟AdidAsset一样用
        if (assetDir.StartsWith("Assets/Resources/"))
        {
            string resPath = assetDir.Substring("Assets/Resources/".Length).TrimEnd('/') + "/" + Path.GetFileNameWithoutExtension(excelFile);
            sb.AppendLine("");
            sb.AppendLine("    static " + data.className + " _self;");
            sb.AppendLine("    public static " + data.className + " self");
            sb.AppendLine("    {");
            sb.AppendLine("        get");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_self == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                _self = Resources.Load<" + data.className + ">(\"" + resPath + "\");");
            sb.AppendLine("            }");
            sb.AppendLine("            return _self;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    #endregion

    #region 数据赋值

    /// <summary>
    /// 把一个格子转成对应类型的值
    /// </summary>
    static object ParseValue(string cell, string csType)
    {
        cell = cell.Trim();
        if (IsListType(csType))
        {
            string elemType = csType.Substring(5, csType.Length - 6);
            Type listType = typeof(List<>).MakeGenericType(CsToSystemType(elemType));
            IList list = (IList)Activator.CreateInstance(listType);
            if (cell != "")
            {
                foreach (string s0 in SplitCsv(StripBrackets(cell)))
                {
                    list.Add(ParseValue(Unquote(s0), elemType));
                }
            }
            return list;
        }
        if (IsDictType(csType))
        {
            string kt, vt;
            GetDictInner(csType, out kt, out vt);
            Type dType = typeof(Dictionary<,>).MakeGenericType(CsToSystemType(kt), CsToSystemType(vt));
            IDictionary dict = (IDictionary)Activator.CreateInstance(dType);
            if (cell != "")
            {
                List<string> parts = SplitCsv(StripBrackets(cell));
                for (int i = 0; i + 1 < parts.Count; i += 2)
                {
                    dict[ParseValue(Unquote(parts[i]), kt)] = ParseValue(Unquote(parts[i + 1]), vt);
                }
                if (parts.Count % 2 == 1) Debug.LogWarning("字典数据是奇数个，最后一个key没有value：" + cell);
            }
            return dict;
        }
        return ParseScalar(cell, csType);
    }

    /// <summary>
    /// class结构：一列的所有非空格子解析成一个值（List类型所有格子都是元素，字典合并所有格子）
    /// </summary>
    static object ParseColumnValue(List<string> cells, string csType)
    {
        if (cells.Count == 0) return ParseValue("", csType);
        if (IsListType(csType))
        {
            string elemType = csType.Substring(5, csType.Length - 6);
            Type listType = typeof(List<>).MakeGenericType(CsToSystemType(elemType));
            IList list = (IList)Activator.CreateInstance(listType);
            foreach (string cell in cells)
            {
                //一个格子里也可能是[..]，元素是基础类型才拆开加
                if (IsListStr(cell) && !IsListType(elemType) && !IsDictType(elemType))
                {
                    foreach (string s0 in SplitCsv(StripBrackets(cell))) list.Add(ParseValue(Unquote(s0), elemType));
                }
                else
                {
                    list.Add(ParseValue(cell, elemType));
                }
            }
            return list;
        }
        if (IsDictType(csType))
        {
            string kt, vt;
            GetDictInner(csType, out kt, out vt);
            Type dType = typeof(Dictionary<,>).MakeGenericType(CsToSystemType(kt), CsToSystemType(vt));
            IDictionary dict = (IDictionary)Activator.CreateInstance(dType);
            foreach (string cell in cells)
            {
                if (IsDictStr(cell))
                {
                    List<string> parts = SplitCsv(StripBrackets(cell));
                    for (int i = 0; i + 1 < parts.Count; i += 2)
                    {
                        dict[ParseValue(Unquote(parts[i]), kt)] = ParseValue(Unquote(parts[i + 1]), vt);
                    }
                }
                else
                {
                    Debug.LogWarning("class结构字典字段的格子要是{k,v,...}：" + cell);
                }
            }
            return dict;
        }
        if (cells.Count > 1) Debug.LogWarning("class结构基础类型字段有多个值，只读第一个：" + csType);
        return ParseValue(cells[0], csType);
    }

    static object ParseScalar(string s, string csType)
    {
        s = s.Trim();
        switch (csType)
        {
            case "string": return s;
            case "int":
                {
                    int iv;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out iv)) return iv;
                    float fv;
                    if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out fv)) return (int)fv;
                    return 0;
                }
            case "float": return ParseFloatSafe(s);
            case "bool": return s == "true" || s == "1";
            case "GameObject": return LoadAsset<GameObject>(s);
            case "Sprite": return LoadAsset<Sprite>(s);
            case "Vector3": return ParseVector3(s);
            case "Vector2": return ParseVector2(s);
        }
        return s;
    }

    static float ParseFloatSafe(string s)
    {
        s = s.Trim();
        float fv;
        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out fv)) return fv;
        if (float.TryParse(s.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out fv)) return fv;
        return 0f;
    }

    static Vector3 ParseVector3(string s)
    {
        string[] parts = StripBrackets(s).Split(',');
        Vector3 v = Vector3.zero;
        if (parts.Length > 0) v.x = ParseFloatSafe(parts[0]);
        if (parts.Length > 1) v.y = ParseFloatSafe(parts[1]);
        if (parts.Length > 2) v.z = ParseFloatSafe(parts[2]);
        return v;
    }

    static Vector2 ParseVector2(string s)
    {
        string[] parts = StripBrackets(s).Split(',');
        Vector2 v = Vector2.zero;
        if (parts.Length > 0) v.x = ParseFloatSafe(parts[0]);
        if (parts.Length > 1) v.y = ParseFloatSafe(parts[1]);
        return v;
    }

    static Type CsToSystemType(string t)
    {
        switch (t)
        {
            case "int": return typeof(int);
            case "float": return typeof(float);
            case "string": return typeof(string);
            case "bool": return typeof(bool);
            case "GameObject": return typeof(GameObject);
            case "Sprite": return typeof(Sprite);
            case "Vector2": return typeof(Vector2);
            case "Vector3": return typeof(Vector3);
        }
        //嵌套类型
        if (IsListType(t)) return typeof(List<>).MakeGenericType(CsToSystemType(t.Substring(5, t.Length - 6)));
        if (IsDictType(t))
        {
            string k, v;
            GetDictInner(t, out k, out v);
            return typeof(Dictionary<,>).MakeGenericType(CsToSystemType(k), CsToSystemType(v));
        }
        return typeof(string);
    }

    /// <summary>
    /// 格子填的是资源路径，空就是null
    /// </summary>
    static T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        if (path == "") return null;
        string p = path.StartsWith("Assets/") ? path : "Assets/" + path;
        T t = AssetDatabase.LoadAssetAtPath<T>(p);
        if (t == null) Debug.LogWarning("找不到资源：" + path);
        return t;
    }

    static Type FindType(string name)
    {
        Type t = Type.GetType(name + ", Assembly-CSharp");
        if (t == null)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(name);
                if (t != null) break;
            }
        }
        return t;
    }

    #endregion

    #region 写回excel

    /// <summary>
    /// 是不是引用类型（GameObject/Sprite，List/Dictionary里带引用的也算）
    /// </summary>
    static bool IsRefType(string csType)
    {
        if (csType == "GameObject" || csType == "Sprite") return true;
        if (IsListType(csType)) return IsRefType(csType.Substring(5, csType.Length - 6));
        if (IsDictType(csType))
        {
            string k, v;
            GetDictInner(csType, out k, out v);
            return IsRefType(v);
        }
        return false;
    }

    /// <summary>
    /// 要不要写回excel：非基础类型（引用类型和Vector2/Vector3，List/Dictionary里带这些的也算）
    /// </summary>
    static bool IsWriteBackType(string csType)
    {
        if (IsRefType(csType)) return true;
        if (csType == "Vector2" || csType == "Vector3") return true;
        if (IsListType(csType)) return IsWriteBackType(csType.Substring(5, csType.Length - 6));
        if (IsDictType(csType))
        {
            string k, v;
            GetDictInner(csType, out k, out v);
            return IsWriteBackType(v);
        }
        return false;
    }

    static string FloatStr(float f)
    {
        return f.ToString("0.####", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 值转成excel格子字符串
    /// </summary>
    static string ToCellString(object val, string csType)
    {
        if (val == null) return "";
        switch (csType)
        {
            case "GameObject":
                {
                    GameObject g = val as GameObject;
                    return g == null ? "" : AssetDatabase.GetAssetPath(g);
                }
            case "Sprite":
                {
                    Sprite s = val as Sprite;
                    return s == null ? "" : AssetDatabase.GetAssetPath(s);
                }
            case "Vector3":
                {
                    Vector3 v = (Vector3)val;
                    return "(" + FloatStr(v.x) + "," + FloatStr(v.y) + "," + FloatStr(v.z) + ")";
                }
            case "Vector2":
                {
                    Vector2 v = (Vector2)val;
                    return "(" + FloatStr(v.x) + "," + FloatStr(v.y) + ")";
                }
            case "int": return ((int)val).ToString();
            case "float": return FloatStr((float)val);
            case "bool": return ((bool)val) ? "true" : "false";
            case "string": return (string)val;
        }
        if (IsListType(csType))
        {
            string inner = csType.Substring(5, csType.Length - 6);
            IList l = (IList)val;
            List<string> parts = new List<string>();
            foreach (object o in l) parts.Add(ToCellString(o, inner));
            return "[" + string.Join(",", parts.ToArray()) + "]";
        }
        if (IsDictType(csType))
        {
            string kt, vt;
            GetDictInner(csType, out kt, out vt);
            IDictionary d = (IDictionary)val;
            List<string> parts = new List<string>();
            foreach (DictionaryEntry kv in d)
            {
                parts.Add(ToCellString(kv.Key, kt));
                parts.Add(ToCellString(kv.Value, vt));
            }
            return "{" + string.Join(",", parts.ToArray()) + "}";
        }
        return val.ToString();
    }

    /// <summary>
    /// key值转字符串，用来和excel第一列对应
    /// </summary>
    static string ValueToKeyString(object val)
    {
        if (val == null) return "";
        if (val is float) return FloatStr((float)val);
        return val.ToString();
    }

    /// <summary>
    /// 把asset目录下所有asset里非基础类型的值写回对应名字的excel
    /// </summary>
    void WriteBackToExcel()
    {
        if (!Directory.Exists(ToAbsolute(assetPath)))
        {
            Debug.LogError("asset目录不存在：" + assetPath);
            return;
        }
        int total = 0;
        foreach (string f in Directory.GetFiles(ToAbsolute(assetPath), "*.asset"))
        {
            if (Path.GetFileName(f).StartsWith("~$")) continue;
            string assetName = Path.GetFileNameWithoutExtension(f);
            string excelFile = ToAbsolute(excelPath) + "/" + assetName + ".xlsx";
            if (!File.Exists(excelFile))
            {
                Debug.LogWarning("没有跟asset同名的excel：" + assetName);
                continue;
            }
            string aPath = "Assets" + f.Substring(Application.dataPath.Length).Replace('\\', '/');
            if (WriteBackOneAsset(aPath, excelFile)) total++;
        }
        AssetDatabase.Refresh();
        Debug.Log("写回excel完成，共写回 " + total + " 个");
    }

    /// <summary>
    /// 一个asset写回它对应的excel，返回是否写了
    /// </summary>
    public static bool WriteBackOneAsset(string assetPath, string excelFile)
    {
        ExcelData data = ParseXlsx(excelFile);
        if (data == null) return false;
        List<ExcelField> fields = ResolveFields(data);
        if (fields == null) return false;
        ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
        if (so == null)
        {
            Debug.LogWarning("没有找到asset：" + assetPath);
            return false;
        }
        //class结构：直接按字段写回
        if (data.structure == "class")
        {
            Dictionary<long, string> ups = new Dictionary<long, string>();
            WriteBackSingleCells(so, data, fields, excelFile, ups);
            if (ups.Count == 0) return false;
            PatchXlsx(excelFile, ups);
            return true;
        }
        FieldInfo listField = so.GetType().GetField("list", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (listField == null)
        {
            Debug.LogWarning(assetPath + " 没有list字段");
            return false;
        }
        IList list = (IList)listField.GetValue(so);
        if (list == null || list.Count == 0) return false;
        Type itemType = listField.FieldType.GetGenericArguments()[0];
        //excel行的key跟asset数据的key对应
        Dictionary<string, int> rowByKey = new Dictionary<string, int>();
        for (int r = 0; r < data.rows.Count; r++)
        {
            string key = data.rows[r][0].Trim();
            if (key != "" && !rowByKey.ContainsKey(key)) rowByKey[key] = r;
        }
        FieldInfo keyField = itemType.GetField(fields[0].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Dictionary<long, string> updates = new Dictionary<long, string>();
        for (int i = 0; i < list.Count; i++)
        {
            object item = list[i];
            string key = keyField == null ? "" : ValueToKeyString(keyField.GetValue(item));
            int row;
            if (!rowByKey.TryGetValue(key, out row))
            {
                Debug.LogWarning(excelFile + " 里没有 key=" + key + " 的行，跳过");
                continue;
            }
            int excelRow = row + 5;
            for (int j = 0; j < fields.Count; j++)
            {
                if (!IsWriteBackType(fields[j].csType)) continue;
                FieldInfo fi = itemType.GetField(fields[j].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi == null) continue;
                updates[excelRow * 16384L + j] = ToCellString(fi.GetValue(item), fields[j].csType);
            }
        }
        if (updates.Count == 0) return false;
        PatchXlsx(excelFile, updates);
        return true;
    }

    /// <summary>
    /// class结构：把asset里非基础类型的值写回对应列，List/Dictionary沿列往下写
    /// </summary>
    static void WriteBackSingleCells(ScriptableObject so, ExcelData data, List<ExcelField> fields, string excelFile, Dictionary<long, string> updates)
    {
        Type type = so.GetType();
        for (int j = 0; j < fields.Count; j++)
        {
            if (!IsWriteBackType(fields[j].csType)) continue;
            FieldInfo fi = type.GetField(fields[j].name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fi == null) continue;
            object val = fi.GetValue(so);
            string csType = fields[j].csType;
            //原来这列有多少格子，写不完的要清掉
            int oldCount = 0;
            foreach (string[] row in data.rows)
            {
                if (row[j].Trim() != "") oldCount++;
            }
            if (IsListType(csType))
            {
                IList l = (IList)val;
                string inner = csType.Substring(5, csType.Length - 6);
                int count = Math.Max(l.Count, oldCount);
                for (int i = 0; i < count; i++)
                {
                    string v = i < l.Count ? ToCellString(l[i], inner) : "";
                    updates[(5 + i) * 16384L + j] = v;
                }
            }
            else if (IsDictType(csType))
            {
                string kt, vt;
                GetDictInner(csType, out kt, out vt);
                IDictionary d = (IDictionary)val;
                List<string> pairs = new List<string>();
                foreach (DictionaryEntry kv in d) pairs.Add(ToCellString(kv.Key, kt) + "," + ToCellString(kv.Value, vt));
                int count = Math.Max(pairs.Count, oldCount);
                for (int i = 0; i < count; i++)
                {
                    string v = i < pairs.Count ? "{" + pairs[i] + "}" : "";
                    updates[(5 + i) * 16384L + j] = v;
                }
            }
            else
            {
                //普通引用类型就写第一个格子
                updates[5 * 16384L + j] = ToCellString(val, csType);
            }
        }
    }

    /// <summary>
    /// 把格子写进xlsx，值空就是删格子。用inlineStr写，不依赖sharedStrings
    /// </summary>
    static void PatchXlsx(string file, Dictionary<long, string> updates)
    {
        try
        {
            using (FileStream fs = OpenXlsx(file, FileAccess.ReadWrite))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                ZipArchiveEntry entry = FindSheetEntry(zip);
                if (entry == null)
                {
                    Debug.LogError("excel里没有sheet：" + file);
                    return;
                }
                string entryName = entry.FullName;
                string xml;
                using (StreamReader sr = new StreamReader(entry.Open(), Encoding.UTF8)) xml = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                PatchSheetCells(doc, updates);
                entry.Delete();
                ZipArchiveEntry ne = zip.CreateEntry(entryName);
                using (StreamWriter sw = new StreamWriter(ne.Open(), Encoding.UTF8)) sw.Write(doc.OuterXml);
            }
            Debug.Log("写回excel：" + file);
        }
        catch (Exception e)
        {
            Debug.LogError("写回excel失败（excel是不是在excel里开着？）：" + file + " " + e.Message);
        }
    }

    static void PatchSheetCells(XmlDocument doc, Dictionary<long, string> updates)
    {
        XmlNode sheetData = doc.SelectSingleNode("//*[local-name()='sheetData']");
        if (sheetData == null) return;
        string ns = doc.DocumentElement.NamespaceURI;
        foreach (KeyValuePair<long, string> kv in updates)
        {
            int row = (int)(kv.Key / 16384);
            int col = (int)(kv.Key % 16384);
            string refStr = ColLetters(col) + row;
            //找已有格子
            XmlNode cell = null;
            foreach (XmlNode c in doc.SelectNodes("//*[local-name()='c']"))
            {
                XmlAttribute ra = c.Attributes["r"];
                if (ra != null && ra.Value == refStr)
                {
                    cell = c;
                    break;
                }
            }
            //空值就是删格子
            if (kv.Value == "")
            {
                if (cell != null) cell.ParentNode.RemoveChild(cell);
                continue;
            }
            if (cell == null)
            {
                //找行，没有就按行号顺序建
                XmlNode rowNode = null;
                foreach (XmlNode rn in sheetData.SelectNodes("*[local-name()='row']"))
                {
                    XmlAttribute rra = rn.Attributes["r"];
                    if (rra != null && rra.Value == row.ToString())
                    {
                        rowNode = rn;
                        break;
                    }
                }
                if (rowNode == null)
                {
                    rowNode = doc.CreateElement("row", ns);
                    XmlAttribute rat = doc.CreateAttribute("r");
                    rat.Value = row.ToString();
                    rowNode.Attributes.Append(rat);
                    XmlNode after = null;
                    foreach (XmlNode rn in sheetData.SelectNodes("*[local-name()='row']"))
                    {
                        if (int.Parse(rn.Attributes["r"].Value) > row)
                        {
                            after = rn;
                            break;
                        }
                    }
                    if (after != null) sheetData.InsertBefore(rowNode, after);
                    else sheetData.AppendChild(rowNode);
                }
                cell = doc.CreateElement("c", ns);
                XmlAttribute cr = doc.CreateAttribute("r");
                cr.Value = refStr;
                cell.Attributes.Append(cr);
                //按列顺序插入
                XmlNode afterC = null;
                foreach (XmlNode cn in rowNode.SelectNodes("*[local-name()='c']"))
                {
                    int nrow, ncol;
                    ParseCellRef(cn.Attributes["r"].Value, out nrow, out ncol);
                    if (ncol > col)
                    {
                        afterC = cn;
                        break;
                    }
                }
                if (afterC != null) rowNode.InsertBefore(cell, afterC);
                else rowNode.AppendChild(cell);
            }
            else
            {
                //注意RemoveAll会把r属性也删了，只能删子节点
                while (cell.FirstChild != null) cell.RemoveChild(cell.FirstChild);
                if (cell.Attributes["t"] != null) cell.Attributes.Remove(cell.Attributes["t"]);
                if (cell.Attributes["s"] != null) cell.Attributes.Remove(cell.Attributes["s"]);
            }
            XmlAttribute tAttr = doc.CreateAttribute("t");
            tAttr.Value = "inlineStr";
            cell.Attributes.Append(tAttr);
            XmlElement isEl = doc.CreateElement("is", ns);
            XmlElement tEl = doc.CreateElement("t", ns);
            tEl.SetAttribute("xml:space", "preserve");
            tEl.InnerText = kv.Value;
            isEl.AppendChild(tEl);
            cell.AppendChild(isEl);
        }
    }

    static string ColLetters(int col)
    {
        string s = "";
        col += 1;
        while (col > 0)
        {
            int m = (col - 1) % 26;
            s = (char)('A' + m) + s;
            col = (col - 1) / 26;
        }
        return s;
    }

    #endregion

    #region 文件操作

    static string ToAbsolute(string assetPath)
    {
        if (assetPath.StartsWith("Assets/")) return Application.dataPath + "/" + assetPath.Substring(7);
        return assetPath;
    }

    static void WriteFile(string assetPath, string content)
    {
        string abs = ToAbsolute(assetPath);
        string dir = Path.GetDirectoryName(abs);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(abs, content, new UTF8Encoding(true));
    }

    static string SanitizeFileName(string name)
    {
        foreach (char ch in Path.GetInvalidFileNameChars()) name = name.Replace(ch, '_');
        return name.Trim().TrimEnd('.');
    }

    #endregion
}

/// <summary>
/// 类代码重新编译后自动生成asset
/// </summary>
[InitializeOnLoad]
static class ReadWriteExcelCompileWatcher
{
    static ReadWriteExcelCompileWatcher()
    {
        EditorApplication.delayCall += OnCompiled;
    }

    static void OnCompiled()
    {
        string files = SessionState.GetString(ReadWriteExcel.SessionFilesKey, "");
        if (files == "") return;
        string assetDir = SessionState.GetString(ReadWriteExcel.SessionAssetDirKey, "");
        SessionState.EraseString(ReadWriteExcel.SessionFilesKey);
        SessionState.EraseString(ReadWriteExcel.SessionAssetDirKey);
        ReadWriteExcel.CreateAllAssets(assetDir, files.Split('\n'));
    }
}
