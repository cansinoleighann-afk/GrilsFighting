from fontTools.subset import Subsetter
from fontTools.ttLib import TTFont

def optimize_font(input_font_path, output_font_path, chars_to_keep):
    # 读取字体文件
    font = TTFont(input_font_path)
    
    # 创建 Subsetter 对象
    subsetter = Subsetter()

    # 将字符列表转换为字符串，并传递给 populate 方法
    subsetter.populate(text=''.join(chars_to_keep))  # 将字符列表合并成字符串

    # 对字体进行子集化处理
    subsetter.subset(font)

    # 保存优化后的字体
    font.save(output_font_path)
    print(f"优化后的字体已保存到 {output_font_path}")

# 示例：只保留字符 'a' 和 'b'
input_font = 'ccc.ttf'  # 输入字体文件路径
output_font = 'BalooBhai-Gujarati_1.ttf'  # 输出字体文件路径

# 定义要保留的字符
#chars_to_keep = ['a', 'b']
chars_to_keep = "1234567890!！%:：;；“”‘’,，.。?？(（)）_-=+/*qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM跳跃自动射击开关视距第一角测试剧情模式对战签到活设置斗任务招募机甲升星士二步从侧边栏进入每天都可以获取金币奖励领礼物匹配中录屏力用继续地图选择随废弃电城怪兽点雪巢始游戏总局赢了杀数平均当前分下个段位青铜东西号首白银钻石次毒刺章节卡推荐己小丑玩命屁是饭的呐喊我方队消灭阵亡表现释梦微信网名性辣条后悔初没你主界面绩确定结束属骑激光枪稳过幕意启高速黄蜂火焰史诗传说暂无频修复时间流浪者出超值包背刷新上场多武器概率铠已损坏提示长按飞行盾闭手臂左解锁查看部装备观否等级介绍通常象征着家在显著和实成道路重要里程碑度挑达改造安合键同售扫荡温馨<#>空格感谢您使本应阅读并《户协议》隐私政策如果拒绝将法更年月日生效勇音乐震退×累计免费送橙色线好大完抽宝箱导弹绵羊有远洋息血量护最能秒充盒不足类型品就功加载呼吸骤停璃倾怜心止柠檬少女幸福屋糕富帅起探索奥秘待变异蛤蟆喷龙爬蝎子狼人蝙蝠刚三头清所蛋守市败王这切换返回页吧胜利失连接至台今且样才未经打原兰博基尼据即携带倒强化简单普困难疯狂站来么快放吗嘿敌还那儿别轻易认输他们真哇事知拳踢需棒球棍圈缺占得展倍抖外插支持吊判断运注错误创建标添桌广告版剔除驻浮层于业参与矩址离收益景体件友盟九宫荣耀华为米屉悬藏片轮播板排算坐请检拟钮终形榜谁必须引范围调投组套区蔽延迟端络什触发享很字登服老报也缓存求直订全恭喜正途戳破去列炮攻环境照明反指预制内雾找脚默太阳源灯声目删该监听容比例符串卓画质给受脑苹盘抬被含材文夹替保径龥极集增像理车拖移偏曲向详热门态摇杆固菜识称统处鼻子骷髅西装男菲尔朋克女孩希拉黑帮大学生特工光头杀手骑手拉腊头套杰森毛线帽绅士黄色热狗鸡露哥混街威"
# 调用优化函数
optimize_font(input_font, output_font, chars_to_keep)
