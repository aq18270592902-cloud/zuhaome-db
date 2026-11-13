using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ZuhaomeFree
{
    internal class Game
    {
        public int Id;
        public string Name;
        public Game(int id, string name) { Id = id; Name = name; }
        public override string ToString() { return Name; }
    }

    internal class RentResult
    {
        public bool Ok;
        public string Error = "";
        public string Acc = "";
        public string Pwd = "";
        public string EndTime = "";
        public string Token = "";
        public int AccountId;
        public bool Pinned;
    }

    internal class Resp
    {
        public int Status;
        public string Err = "";
        public Dictionary<string, object> Body;
    }

    internal class MainForm : Form
    {
        private const string Api = "https://api.zuhaome.com/api/v1";
        private static readonly HttpClient Http = NewClient();
        private static Mutex _one;

        private static readonly Game[] Hot = new Game[]
        {
            new Game(2, "消逝的光芒2"),
            new Game(3, "极限竞速：地平线4"),
            new Game(4, "地痞街区"),
            new Game(5, "泰拉瑞亚"),
            new Game(6, "夜族崛起"),
            new Game(7, "帝国时代2：决定版"),
            new Game(8, "正当防卫4（带DLC）"),
            new Game(9, "欧洲卡车模拟2"),
            new Game(10, "恐怖黎明"),
            new Game(11, "幸福工厂"),
            new Game(12, "幻想生活i"),
            new Game(13, "方舟：生存进化"),
            new Game(14, "揍击派对"),
            new Game(15, "挺进地牢"),
            new Game(16, "幽灵行动：荒野"),
            new Game(17, "异星探险家"),
            new Game(18, "森林 The Forest"),
            new Game(19, "黑暗之魂3+豪华版DLC"),
            new Game(20, "光与影：33号远征队"),
            new Game(21, "骗子酒吧"),
            new Game(22, "隐形守护者+DLC"),
            new Game(23, "人类一败涂地"),
            new Game(24, "胡闹厨房2+全DLC"),
            new Game(25, "全面战争：三国+豪华版DLC"),
            new Game(27, "绿色地狱"),
            new Game(28, "仁王1"),
            new Game(29, "空洞骑士"),
            new Game(30, "传送门2"),
            new Game(31, "生态环保ECO"),
            new Game(32, "猎人：荒野的召唤"),
            new Game(33, "僵尸部队4"),
            new Game(34, "胡闹厨房2"),
            new Game(35, "PEAK/拉我一把"),
            new Game(36, "恐鬼症"),
            new Game(37, "战锤40K：暗潮"),
            new Game(38, "战锤40K：星际战士2"),
            new Game(39, "赛博朋克2077+往日之影DLC"),
            new Game(40, "末日的第53天"),
            new Game(41, "英灵神殿"),
            new Game(42, "灵魂摆渡人"),
            new Game(43, "消逝的光芒1"),
            new Game(44, "漫漫长夜"),
            new Game(45, "双人成行"),
            new Game(46, "毁灭战士4 DOOM"),
            new Game(47, "流放者柯南：增强版"),
            new Game(48, "求生之路2"),
            new Game(49, "恐惧之间（高时长）"),
            new Game(50, "绝地潜兵2"),
            new Game(51, "木筏求生"),
            new Game(52, "艾尔登法环：黑夜君临"),
            new Game(53, "叛乱：沙漠风暴"),
            new Game(54, "禁闭求生2+创始人DLC"),
            new Game(55, "幻兽帕鲁"),
            new Game(56, "最终幻想16"),
            new Game(57, "毁灭战士：黑暗时代"),
            new Game(58, "黑暗之魂2：原罪学者"),
            new Game(60, "第一狂战士：卡赞"),
            new Game(63, "仁王2"),
            new Game(64, "最终幻想7：重生"),
            new Game(66, "对马岛之魂导演剪辑版"),
            new Game(68, "最终幻想7：重制版 1."),
            new Game(70, "匹诺曹的谎言"),
            new Game(72, "战神5：诸神黄昏"),
            new Game(73, "暗喻幻想"),
            new Game(74, "泡姆泡姆"),
            new Game(75, "灵魂面甲"),
            new Game(76, "潜行者2：切尔诺贝利之心"),
            new Game(78, "边缘世界"),
            new Game(79, "卧龙：苍天陨落"),
            new Game(80, "漫威蜘蛛侠2"),
            new Game(82, "狂热运输2"),
            new Game(83, "漫威蜘蛛侠1"),
            new Game(84, "最终幻想15"),
            new Game(85, "神界：原罪2"),
            new Game(86, "逃脱者2+年度版DLC"),
            new Game(87, "最后生还者1"),
            new Game(88, "潜水员戴夫"),
            new Game(90, "方舟：生存飞升"),
            new Game(91, "黑暗之魂：重制版"),
            new Game(92, "维多利亚3"),
            new Game(94, "十字军之王3"),
            new Game(95, "博德之门3"),
            new Game(96, "都市：天际线2"),
            new Game(97, "女神异闻录5皇家版"),
            new Game(98, "毁灭战士：永恒"),
            new Game(99, "无人深空"),
            new Game(102, "超级兔子人"),
            new Game(103, "死亡岛：终极版"),
            new Game(104, "霍格沃茨之遗+豪华版DLC"),
            new Game(105, "全面战争：战锤3"),
            new Game(106, "原子之心"),
            new Game(107, "明末：渊虚之羽"),
            new Game(108, "真三国无双：起源"),
            new Game(109, "剑星+完整版DLC"),
            new Game(110, "雾锁王国"),
            new Game(111, "鬼泣5"),
            new Game(112, "艾尔登法环+黄金树幽影DLC"),
            new Game(113, "天国：拯救2"),
            new Game(114, "致命公司"),
            new Game(115, "帝国时代4"),
            new Game(116, "街头霸王6"),
            new Game(117, "怪物猎人：荒野"),
            new Game(118, "怪物猎人：崛起"),
            new Game(119, "猛兽派对 豪华版"),
            new Game(120, "骑马与砍杀2：霸主+战帆DLC"),
            new Game(121, "深岩银河"),
            new Game(122, "人间地狱"),
            new Game(123, "禁闭求生1"),
            new Game(124, "大富翁11"),
            new Game(125, "双影奇境"),
            new Game(126, "森林之子【假】"),
            new Game(127, "逃离后室"),
            new Game(128, "底特律：化身为人"),
            new Game(129, "只狼：影逝二度"),
            new Game(130, "吞噬"),
            new Game(131, "枪火重生"),
            new Game(132, "怪物猎人：世界+冰原DLC"),
            new Game(133, "极限竞速：地平线5"),
            new Game(134, "黑神话：悟空 豪华版"),
            new Game(136, "生化危机4重制版"),
            new Game(137, "帝国时代4+十字与玫瑰骑士DLC"),
            new Game(138, "我们到了吗？"),
            new Game(139, "使命召唤：现代战争重制版"),
            new Game(140, "使命召唤现代战争重制版多人"),
            new Game(142, "冰与火之舞"),
            new Game(143, "赛博朋克 2077"),
            new Game(144, "《新生也疯狂》第1季"),
            new Game(145, "My Bimbo Dream：第一季"),
            new Game(149, "城市：天际线"),
            new Game(150, "你画我猜"),
            new Game(151, "植物大战僵尸年度版"),
            new Game(152, "虐杀原形"),
            new Game(153, "四海兄弟2经典版"),
            new Game(154, "饥荒"),
            new Game(155, "外科医生模拟器"),
            new Game(156, "茶杯头"),
            new Game(157, "我是面包"),
            new Game(158, "四海兄弟3最终版"),
            new Game(159, "饥饿派画家"),
            new Game(160, "雨中冒险2"),
            new Game(161, "逃脱者2"),
            new Game(162, "霓虹深渊"),
            new Game(163, "鹅作剧"),
            new Game(164, "流浪汉模拟器"),
            new Game(167, "四海兄弟II 最终版"),
            new Game(168, "《四海兄弟: 最终版》​"),
            new Game(169, "猛兽派对"),
            new Game(170, "小偷模拟器2"),
            new Game(174, "席德梅尔的文明6"),
            new Game(175, "三国志汉末霸业"),
            new Game(176, "传送门"),
            new Game(177, "文明5"),
            new Game(178, "镜之边缘"),
            new Game(179, "巫师增强版"),
            new Game(180, "巫师2国王刺客增强版"),
            new Game(181, "地铁最后的曙光完全版"),
            new Game(182, "夺命邮差2"),
            new Game(183, "逃生1"),
            new Game(184, "中土世界：暗影魔多"),
            new Game(185, "巫师3：狂猎 完全版"),
            new Game(186, "饥荒联机版"),
            new Game(187, "中土世界：战争之影"),
            new Game(189, "方舟适者生存"),
            new Game(190, "星露谷物语"),
            new Game(191, "逃生2"),
            new Game(192, "史莱姆牧场"),
            new Game(194, "战锤40K：格雷迪厄斯"),
            new Game(195, "过山车之星"),
            new Game(196, "艾迪芬奇的记忆"),
            new Game(198, "太空堡垒卡拉狄加死锁"),
            new Game(201, "又一个僵尸防御HD"),
            new Game(202, "小偷模拟器"),
            new Game(203, "心魔"),
            new Game(205, "嗜血印"),
            new Game(208, "奥日与萤火意志"),
            new Game(210, "机械博物馆"),
            new Game(214, "骑马与砍杀2：霸主"),
            new Game(215, "荒野八人组"),
            new Game(216, "太空狼人杀"),
            new Game(217, "美足按摩师"),
            new Game(220, "孢子"),
            new Game(221, "城堡破坏者"),
            new Game(222, "收获日2"),
            new Game(223, "王国保卫战"),
            new Game(224, "节奏地牢"),
            new Game(225, "7日杀"),
            new Game(226, "石油骚动"),
            new Game(227, "传说之下"),
            new Game(228, "进击要塞"),
            new Game(229, "失落城堡"),
            new Game(230, "王国保卫战前线"),
            new Game(231, "终极钓鱼模拟器"),
            new Game(232, "为了吾王"),
            new Game(233, "脑叶公司"),
            new Game(234, "怪物猎人：世界"),
            new Game(235, "武器店物语"),
            new Game(236, "潜渊症"),
            new Game(237, "魔镜"),
            new Game(238, "不要喂猴子"),
            new Game(239, "大富翁"),
            new Game(240, "奔跑吧僵尸的食物"),
            new Game(241, "王国：两位君主"),
            new Game(242, "浮岛物语"),
            new Game(243, "龙崖"),
            new Game(244, "喵斯快跑"),
            new Game(245, "王国保卫战起源"),
            new Game(246, "环形帝国"),
            new Game(247, "喵咪斗恶龙2"),
            new Game(248, "西米岛"),
            new Game(250, "环形帝国对决"),
            new Game(251, "《文字游戏》"),
            new Game(252, "漂泊牧歌 (The Wandering Village)"),
            new Game(253, "小骨英雄杀手"),
            new Game(255, "南瓜杰克"),
            new Game(256, "觅长生"),
            new Game(257, "Legend Creatures(传奇生物)"),
            new Game(258, "世界盒子上帝模拟器"),
            new Game(259, "药剂工艺：炼金模拟器"),
            new Game(261, "暴君的游戏"),
            new Game(262, "哥布林弹球 Peglin"),
            new Game(263, "异形工厂（Shapez）"),
            new Game(264, "王国保卫战复仇"),
            new Game(265, "团战经理"),
            new Game(267, "一起来造动物园"),
            new Game(268, "史莱姆牧场2"),
            new Game(269, "通神榜"),
            new Game(271, "吸血鬼幸存者"),
            new Game(273, "堆叠大陆"),
            new Game(275, "苦主救助中心Ⅰ"),
            new Game(276, "中华一商"),
            new Game(277, "不双修就去世"),
            new Game(278, "犹格索托斯的庭院"),
            new Game(279, "前方高能"),
            new Game(280, "逃离鸭科夫"),
            new Game(281, "半条命2"),
            new Game(282, "半条命2死亡竞赛"),
            new Game(283, "半条命死亡竞赛起源"),
            new Game(284, "求生之路1"),
            new Game(285, "To the Moon《去月球》"),
            new Game(286, "造物主"),
            new Game(287, "正当防卫3"),
            new Game(289, "狙击精英4"),
            new Game(290, "狙击手幽灵战士3"),
            new Game(292, "谁是你爸爸"),
            new Game(294, "全面战争模拟器"),
            new Game(296, "战地乌鸦"),
            new Game(298, "速降王者"),
            new Game(300, "御宅幻想"),
            new Game(301, "御宅幻想2"),
            new Game(304, "杀手2"),
            new Game(305, "同步音律喵赛克"),
            new Game(306, "禁忌裂痕"),
            new Game(307, "知只大冒险"),
            new Game(308, "大富翁10"),
            new Game(309, "盗贼之海"),
            new Game(311, "梦中女孩"),
            new Game(312, "创世理想乡"),
            new Game(315, "鬼谷八荒"),
            new Game(316, "萌猫公园"),
            new Game(317, "兽人必须死3"),
            new Game(318, "王座陨落"),
            new Game(319, "Dystopika: 未来城市沙盒"),
            new Game(320, "星际工业国"),
            new Game(321, "米塔 MiSide"),
            new Game(323, "攀爬动物：在一起"),
            new Game(324, "恶魔轮盘"),
            new Game(326, "链在一起"),
            new Game(330, "潜行者切尔诺贝利的阴影"),
            new Game(331, "潜行者晴空"),
            new Game(332, "潜行者：普里皮亚季的召唤"),
            new Game(333, "这是我的战争"),
            new Game(334, "地铁2033 重制版"),
            new Game(337, "战锤：末世鼠疫2"),
            new Game(338, "控制：终极合辑"),
            new Game(339, "狙击手：幽灵战士契约"),
            new Game(340, "桥梁建筑师2"),
            new Game(341, "森林之子"),
            new Game(342, "狙击手：幽灵战士契约2"),
            new Game(343, "地铁逃离增强版"),
            new Game(344, "《潜行者：切尔诺贝利的阴影》增强版"),
            new Game(345, "《潜行者：晴空》增强版"),
            new Game(346, "《潜行者：普里皮亚季的召唤》增强版"),
            new Game(347, "生化奇兵：无限 完全版"),
            new Game(348, "黑神话：悟空"),
            new Game(349, "夜袭者 Night Raider"),
            new Game(350, "检疫区：最后一站"),
            new Game(351, "剑星"),
            new Game(352, "《命令与征服™：红色警戒 3》"),
            new Game(353, "生化危机6"),
            new Game(354, "黑暗之魂3"),
            new Game(355, "古墓丽影：崛起"),
            new Game(356, "生化危机3"),
            new Game(357, "生化危机：抵抗"),
            new Game(358, "僵尸毁灭工程"),
            new Game(359, "蝙蝠俠：阿卡漢騎士"),
            new Game(360, "火影忍者终极风暴4"),
            new Game(361, "吸血鬼"),
            new Game(362, "古墓丽影：暗影"),
            new Game(363, "真人快打11"),
            new Game(364, "漫威银河护卫队"),
            new Game(365, "千恋＊万花"),
            new Game(366, "摇曳女孩"),
            new Game(367, "主播女孩重度依赖"),
            new Game(368, "火山的女儿"),
            new Game(372, "一起开超市"),
            new Game(373, "无主之地2"),
            new Game(374, "解限机"),
            new Game(375, "虚拟桌宠模拟器"),
            new Game(376, "邦戈猫"),
            new Game(377, "反恐精英2"),
            new Game(378, "战争雷霆"),
            new Game(382, "台球国度FX"),
            new Game(383, "《守望先锋®》"),
            new Game(384, "生化危机5"),
            new Game(385, "生化危机：启示录"),
            new Game(386, "生化危机4 (2005)"),
            new Game(387, "深海迷航"),
            new Game(388, "格兰蒂亚秘闻"),
            new Game(389, "群星"),
            new Game(390, "生化危机启示录2"),
            new Game(391, "生化危机"),
            new Game(392, "深入INSIDE"),
            new Game(393, "上古卷轴OL"),
            new Game(394, "死亡岛复古复仇"),
            new Game(395, "生化危机0"),
            new Game(396, "保持通话没人爆炸"),
            new Game(397, "黎明杀机"),
            new Game(398, "死亡岛激流决定版"),
            new Game(400, "玩具熊的五夜后宫4"),
            new Game(402, "小马岛"),
            new Game(403, "命运石之门"),
            new Game(404, "弹丸论破1"),
            new Game(406, "生化危机7"),
            new Game(407, "小小梦魇"),
            new Game(411, "胡闹厨房"),
            new Game(412, "缺氧"),
            new Game(413, "量子破碎"),
            new Game(414, "虚构世界"),
            new Game(415, "六角疑云"),
            new Game(416, "新枪弹辩驳V3"),
            new Game(417, "乞丐模拟器"),
            new Game(418, "苏醒之路"),
            new Game(419, "杀戮尖塔"),
            new Game(420, "我们也曾在这里"),
            new Game(421, "中国式家长"),
            new Game(423, "小小梦魇2"),
            new Game(424, "我们一起在这里"),
            new Game(425, "生化危机2"),
            new Game(426, "野狼迷踪：周年纪念版"),
            new Game(427, "超凡双生"),
            new Game(428, "霍格沃茨之遗"),
            new Game(429, "火柴人亨利合集"),
            new Game(430, "碧蓝幻想Versus"),
            new Game(431, "邪恶冥刻"),
            new Game(432, "港诡实录"),
            new Game(433, "恐俄症"),
            new Game(434, "生化危机：村庄/生化危机8"),
            new Game(435, "橡胶强盗"),
            new Game(436, "生化危机：对抗"),
            new Game(437, "逃生：试炼"),
            new Game(438, "咩咩启示录"),
            new Game(439, "迷失 Stray"),
            new Game(440, "我们曾到此永"),
            new Game(441, "中国式网游"),
            new Game(443, "行尸走肉Telltale终极系列"),
            new Game(444, "人格解体"),
            new Game(445, "孤山独影"),
            new Game(447, "女鬼桥 开魂路"),
            new Game(448, "穹顶守护者"),
            new Game(449, "双点校园"),
            new Game(450, "为了吾王2"),
            new Game(451, "死亡空间"),
            new Game(452, "零 ～濡鸦之巫女～"),
            new Game(453, "节奏天鸽"),
            new Game(454, "王牌与冒险"),
            new Game(455, "幽灵观察者"),
            new Game(456, "土豆兄弟"),
            new Game(457, "黎明前20分钟"),
            new Game(458, "我的小柯基"),
            new Game(459, "中国式相亲"),
            new Game(460, "生灵重塑"),
            new Game(461, "小小梦魇 强化版"),
            new Game(462, "诅咒2"),
            new Game(463, "我们曾到此探险:友谊考验"),
            new Game(464, "小丑牌"),
            new Game(465, "It Has My Face 镜杀"),
            new Game(466, "枪途末路"),
            new Game(467, "幽灵尖叫"),
            new Game(469, "迷魅狩猎"),
            new Game(470, "密室逃脱模拟器2"),
            new Game(472, "沙威玛传奇"),
            new Game(473, "我在地府打麻将"),
            new Game(474, "卡牌店模拟器 多人联机版"),
            new Game(477, "奇幻将军2"),
            new Game(478, "真实台球3D - Pool"),
            new Game(481, "皮克公园经典版"),
            new Game(482, "鹅鸭杀"),
            new Game(484, "全面战争：幕府将军2"),
            new Game(485, "英雄连2"),
            new Game(486, "火星求生"),
            new Game(487, "战舰世界"),
            new Game(488, "异形丛生：虫启天降 (Alien Swarm: Reactive Drop)"),
            new Game(489, "银河破裂者"),
            new Game(490, "无畏战舰"),
            new Game(491, "王国纪元 (Lords Mobile)"),
            new Game(493, "光环无限"),
            new Game(494, "传奇商店"),
            new Game(496, "我的上司有点不正常"),
            new Game(497, "Z1大逃杀"),
            new Game(498, "无限法则"),
            new Game(499, "命运2"),
            new Game(500, "灵境奇谈"),
            new Game(501, "两个世界2高清版"),
            new Game(502, "骑马与砍杀：战团"),
            new Game(503, "钢铁苍穹入侵"),
            new Game(505, "仇杀乌鸦之诅咒"),
            new Game(506, "911接线员"),
            new Game(511, "狂野泊车 / You Suck at Parking®"),
            new Game(512, "喋血复仇"),
            new Game(513, "艾尔登法环"),
            new Game(514, "时空错乱之旅"),
            new Game(515, "飞车派对：乱斗高尔夫"),
            new Game(516, "JOJO的奇妙冒险 群星之战 重制版"),
            new Game(517, "薇妮巴一家"),
            new Game(519, "飞马座远征"),
            new Game(520, "谁把马库斯叔叔静音了"),
            new Game(522, "花园生涯：模拟佛系生活"),
            new Game(523, "纳赫鲁博王国地下城大师"),
            new Game(524, "你的老母"),
            new Game(526, "黑暗逃生"),
            new Game(527, "俄罗斯钓鱼4"),
            new Game(530, "黑色小队"),
            new Game(531, "神佑释放"),
            new Game(532, "情感反诈模拟器"),
            new Game(533, "不义联盟人间之神终极版"),
            new Game(534, "我叔叔的故事"),
            new Game(535, "九张羊皮纸"),
            new Game(536, "奇幻滑雪VR"),
            new Game(537, "死亡细胞"),
            new Game(538, "遗迹：灰烬重生"),
            new Game(539, "燥热VR"),
            new Game(540, "节奏光剑"),
            new Game(541, "枪的故事防弹VR"),
            new Game(542, "瞳：祈愿 VR / Pupil: Wandering VR"),
            new Game(544, "重明鸟 Bright Bird"),
            new Game(545, "斩妖Raksasi"),
            new Game(546, "哈迪斯"),
            new Game(547, "暖雪"),
            new Game(548, "完美音浪"),
            new Game(549, "绝对魔权"),
            new Game(550, "贪婪大地"),
            new Game(551, "Halls of Torment 苦痛殿堂"),
            new Game(552, "苍翼:混沌效应"),
            new Game(554, "Warframe 星际战甲"),
            new Game(557, "彩虹六号：围攻"),
            new Game(560, "天空之山"),
            new Game(561, "女巫来了"),
            new Game(563, "Warm Village 暖暖村物语"),
            new Game(564, "太吾绘卷：天幕心帷"),
            new Game(565, "隐形守护者"),
            new Game(566, "Christmas Tina -泡沫冬景-"),
            new Game(567, "基佬大乱斗"),
            new Game(568, "油管主播的生活"),
            new Game(569, "正当防卫4"),
            new Game(570, "禁闭求生2"),
            new Game(571, "請出示文件"),
            new Game(572, "细胞生存战"),
            new Game(573, "尼尔：机械纪元"),
            new Game(574, "丸霸无双"),
            new Game(575, "超阈限空间 Superliminal"),
            new Game(577, "星球工匠Demo"),
            new Game(579, "全面战争：三国"),
            new Game(580, "盖瑞模组"),
            new Game(581, "收获日掠夺"),
            new Game(582, "武装突袭2"),
            new Game(583, "武装突袭2箭头行动"),
            new Game(584, "武装突袭2箭头行动"),
            new Game(585, "死亡岛流行病"),
            new Game(586, "地狱已满"),
            new Game(587, "武装突袭2DayZ模组"),
            new Game(588, "僵尸战争新开端"),
            new Game(589, "猎人经典版"),
            new Game(591, "宝藏世界"),
            new Game(592, "未转变者"),
            new Game(594, "古墓丽影"),
            new Game(595, "疯狂麦克斯"),
            new Game(596, "美国卡车模拟器"),
            new Game(597, "冰汽时代"),
            new Game(598, "影子武士2"),
            new Game(599, "围攻"),
            new Game(600, "钢铁雄心4"),
            new Game(601, "恶行"),
            new Game(603, "世界汽车拉力锦标赛7"),
            new Game(606, "致命捕捞"),
            new Game(607, "避风港"),
            new Game(608, "终极钓鱼模拟器2"),
            new Game(609, "幽灵行者"),
            new Game(610, "断钢"),
            new Game(611, "易红2"),
            new Game(612, "Chasing Tails 〜雪中誓约〜"),
            new Game(613, "杀手：暗杀世界"),
            new Game(615, "命令与征服：重制版"),
            new Game(616, "混乱制造者"),
            new Game(617, "Fling to the Finish 终极拉扯"),
            new Game(619, "超市模拟器Super"),
            new Game(620, "快餐店模拟器"),
            new Game(622, "世界一級方程式錦標賽2018"),
            new Game(625, "LoveChoice 拣爱"),
            new Game(626, "饿狼传说：群狼之城"),
            new Game(628, "超级鸡马"),
            new Game(629, "妄想症 Deliver Me"),
            new Game(630, "你妈"),
            new Game(631, "终极史诗战争模拟器"),
            new Game(632, "亿万僵尸"),
            new Game(633, "僵尸世界大战"),
            new Game(634, "超能队长"),
            new Game(635, "终极史诗战争模拟器2"),
            new Game(636, "暗黑地牢"),
            new Game(637, "雨世界"),
            new Game(638, "上古卷轴5天际特别版"),
            new Game(639, "了不起的修仙模拟器"),
            new Game(640, "活侠传"),
            new Game(641, "镇邪 Zhenxie"),
            new Game(642, "无尽地牢"),
            new Game(643, "终极僵尸防御"),
            new Game(644, "夏日狂想曲：乡间的难忘回忆"),
            new Game(645, "桥梁建造师"),
            new Game(646, "天国拯救"),
            new Game(647, "腐烂国度2圣剑版"),
            new Game(648, "沙石镇时光"),
            new Game(649, "一方灵田"),
            new Game(650, "纸飞机"),
            new Game(651, "半条命"),
            new Game(652, "模拟火车经典"),
            new Game(653, "丧尸围城2"),
            new Game(654, "终极街头霸王4"),
            new Game(655, "地狱边境"),
            new Game(656, "闪克2"),
            new Game(657, "堡垒"),
            new Game(658, "异形隔离"),
            new Game(659, "坎巴拉太空计划"),
            new Game(660, "DMC鬼泣"),
            new Game(661, "拳皇13Steam版"),
            new Game(662, "桌面地下城"),
            new Game(663, "勿忘我"),
            new Game(664, "神界原罪经典版"),
            new Game(665, "剑士"),
            new Game(666, "出击飞龙"),
            new Game(667, "世界汽车拉力锦标赛4"),
            new Game(668, "恶灵附身"),
            new Game(669, "破碎大陆年度版"),
            new Game(670, "武装原型"),
            new Game(671, "百分百橙汁"),
            new Game(672, "合金装备5幻痛"),
            new Game(673, "劳拉与奥西里斯神庙"),
            new Game(674, "街头霸王5"),
            new Game(675, "合金装备5原爆点"),
            new Game(676, "鬼泣4特别版"),
            new Game(677, "洛克人遗产合集"),
            new Game(678, "龙之信条：黑暗觉者"),
            new Game(679, "神界：原罪增强版"),
            new Game(681, "无主之地3"),
            new Game(682, "乌波决定版"),
            new Game(683, "耻辱2"),
            new Game(684, "地狱之刃塞娜的献祭"),
            new Game(685, "猎天使魔女"),
            new Game(686, "北境之地：决定版"),
            new Game(687, "洛克人传奇合集2"),
            new Game(688, "王国新大陆"),
            new Game(689, "蔚蓝"),
            new Game(691, "街头霸王30周年纪念合集"),
            new Game(692, "脑航员2"),
            new Game(693, "不义联盟2"),
            new Game(694, "波西亚时光"),
            new Game(695, "噬血代码"),
            new Game(696, "仙剑奇侠传四"),
            new Game(697, "仙剑奇侠传五前传"),
            new Game(698, "三位一体4梦魇王子"),
            new Game(699, "密教模拟器"),
            new Game(700, "破晓传奇"),
            new Game(701, "洛克人X遗产合集"),
            new Game(702, "洛克人X遗产合集2"),
            new Game(703, "地狱之刃塞娜的献祭VR版"),
            new Game(704, "绯红结系"),
            new Game(705, "忍者印记重制版"),
            new Game(706, "女巫"),
            new Game(707, "王权：权力的游戏"),
            new Game(708, "趣拼拼：肖像画 Pleasure Puzzle:Portrait"),
            new Game(709, "英勇无厌"),
            new Game(710, "最后的篝火"),
            new Game(711, "圣女战旗"),
            new Game(712, "空洞骑士：丝之歌"),
            new Game(713, "污泥人生"),
            new Game(714, "航海日记：起航"),
            new Game(715, "人中之龙7　光与暗的去向　国际版"),
            new Game(716, "侏罗纪世界进化2"),
            new Game(717, "Steelrising 钢之崛起"),
            new Game(718, "戴森球计划"),
            new Game(719, "静脉注射"),
            new Game(721, "东方夜雀食堂"),
            new Game(722, "伊始之地"),
            new Game(723, "战神4"),
            new Game(724, "地狱仆从"),
            new Game(726, "卡牌生存：热带岛屿"),
            new Game(727, "小缇娜强袭龙堡"),
            new Game(728, "魔镜2X计划"),
            new Game(729, "城堡之王"),
            new Game(730, "奇怪的RPG"),
            new Game(731, "神话时代重述"),
            new Game(732, "洞察之岛"),
            new Game(733, "地中海地獄"),
            new Game(734, "冒险家阿曼达"),
            new Game(735, "卡牌城镇 Cardboard Town"),
            new Game(736, "背包乱斗：福西法的宝藏"),
            new Game(737, "骰子浪游者"),
            new Game(738, "王之凝视"),
            new Game(739, "小动物餐吧"),
            new Game(740, "猫神牧场"),
            new Game(741, "杀戮尖塔2"),
            new Game(743, "无用之人：登神长阶"),
            new Game(744, "种呱得呱 Rana Card"),
            new Game(745, "咔砰！玩具乱斗屋"),
            new Game(746, "蝙蝠侠阿卡姆疯人院年度版"),
            new Game(747, "蝙蝠侠阿卡姆之城年度版"),
            new Game(748, "乐高旋风忍者大电影"),
            new Game(749, "乐高漫威超级英雄2"),
            new Game(750, "尘埃拉力赛2.0"),
            new Game(751, "邪恶天才2"),
            new Game(752, "七龙珠Z 卡卡洛特"),
            new Game(753, "世界汽车拉力锦标赛9"),
            new Game(754, "人魅"),
            new Game(755, "食人鲨"),
            new Game(756, "深海迷航：冰点之下"),
            new Game(757, "妹相随～黑白世界的缤纷冒险～"),
            new Game(758, "饿殍：明末千里行"),
            new Game(760, "掘地求财"),
            new Game(761, "玩具熊的五夜后宫"),
            new Game(762, "白色情人节：恐怖学校"),
            new Game(763, "Mad Father《狂父》"),
            new Game(764, "弹丸论破：绝对绝望少女"),
            new Game(765, "我和她的世界末日"),
            new Game(767, "魔女之家MV"),
            new Game(769, "亚托莉我挚爱的时光"),
            new Game(770, "谜语小丑"),
            new Game(771, "心跳文学部Plus"),
            new Game(772, "怠惰的怪兽公主不想工作"),
            new Game(773, "星光咖啡馆与死神之蝶"),
            new Game(774, "恐怖美术馆"),
            new Game(775, "暴食的怪兽公主：惑星美食之旅"),
            new Game(776, "傲慢的怪兽公主与名侦探使魔"),
            new Game(777, "命运之夜重制版"),
            new Game(778, "拔作岛"),
            new Game(779, "魔女的夜宴"),
            new Game(780, "冬日狂想曲"),
            new Game(782, "拔作岛2"),
            new Game(783, "坤梦惊魂"),
            new Game(784, "极道无赖:横滨按摩店篇"),
            new Game(785, "不/存在的你，和我"),
            new Game(786, "极品采花郎"),
            new Game(787, "乡间拾趣夏日谈"),
            new Game(788, "魔法少女的魔女审判"),
            new Game(789, "你的老婆"),
            new Game(790, "与不登校妹妹一起的30天"),
            new Game(791, "伊藤润二狂热 : 无止的囹圄"),
            new Game(793, "家有恶邻"),
            new Game(794, "小兵步枪"),
            new Game(795, "闹鬼房子恐怖镇"),
            new Game(796, "疯狂派对"),
            new Game(797, "智慧之海"),
            new Game(798, "锈湖旅馆"),
            new Game(799, "龙骑士"),
            new Game(800, "地狱少女"),
            new Game(802, "人中之龙０　誓约的场所"),
            new Game(803, "三色绘恋"),
            new Game(804, "暗黑血统创世纪"),
            new Game(805, "恶果之地"),
            new Game(806, "Reformers Intl Ver(变革者国际版)"),
            new Game(809, "暴雨"),
            new Game(811, "狂野星球之旅"),
            new Game(813, "拆箱"),
            new Game(814, "熊与早餐"),
            new Game(815, "哈迪斯2"),
            new Game(816, "无尽旅图"),
            new Game(817, "西部甜心"),
            new Game(818, "胡闹厨房 全都好吃"),
            new Game(819, "烟火"),
            new Game(820, "代号探戈"),
            new Game(821, "饼干点点乐"),
            new Game(822, "渔帆暗涌"),
            new Game(823, "林中小女巫"),
            new Game(824, "因果动物园"),
            new Game(826, "死亡搁浅：导演剪辑版"),
            new Game(827, "负鼠闹翻天"),
            new Game(828, "柯娜精神之桥"),
            new Game(829, "喜丧"),
            new Game(830, "方寸文明"),
            new Game(831, "Mudborne：育蛙模拟器"),
            new Game(832, "鸡械绿洲"),
            new Game(833, "菜市场模拟器"),
            new Game(834, "舒舒服服小岛时光"),
            new Game(835, "虫虫生态箱"),
            new Game(836, "死亡日：狂杀末路"),
            new Game(837, "鼠鼠登月 Rocket Rats"),
            new Game(838, "电子幻术"),
            new Game(839, "遗忘週一咖啡馆"),
            new Game(840, "动物栏：桌面牧场"),
            new Game(841, "原点计划：血肉工匠"),
            new Game(842, "你有饼吗？"),
            new Game(843, "gogh: 虚拟形象与专注"),
            new Game(844, "疯菇突围 (Funguys Swarm)"),
            new Game(845, "欢乐水族馆"),
            new Game(846, "纸房子"),
            new Game(847, "小女巫幸存者"),
            new Game(848, "頭七"),
            new Game(850, "龟龟潜海记 Shelldiver"),
            new Game(851, "鼠之城邦"),
            new Game(852, "超级键盘侠"),
            new Game(853, "深海惊魂"),
            new Game(855, "手部模拟器"),
            new Game(856, "厕所穿越记"),
            new Game(857, "荒野之地2：新大陆"),
            new Game(858, "寂静岭2"),
            new Game(859, "《命令与征服：红色警戒 2 及尤里的复仇》"),
            new Game(860, "十三号星期五"),
            new Game(861, "杀戮空间2"),
            new Game(862, "掘地求升"),
            new Game(863, "落日余晖"),
            new Game(864, "打造世界"),
            new Game(865, "无尽帝国"),
            new Game(866, "一起玩农场"),
            new Game(867, "动物园之星"),
            new Game(868, "金垦小镇"),
            new Game(869, "牧野之歌"),
            new Game(870, "海岛之魂"),
            new Game(871, "护核纪元"),
            new Game(873, "露玛岛"),
            new Game(874, "一起玩农场2"),
            new Game(876, "双点医院"),
            new Game(877, "人群模拟器"),
            new Game(879, "亚尔默逊"),
            new Game(880, "热血无赖终极版"),
            new Game(881, "GRIP战斗赛车"),
            new Game(883, "星际小队 / Space Crew: Legendary Edition"),
            new Game(884, "Bodycam 真实射击"),
            new Game(885, "完蛋！我也能追到美女了！"),
            new Game(886, "厨房战争"),
            new Game(889, "城堡突围"),
            new Game(891, "狙击精英5"),
            new Game(892, "战斗砖块剧场"),
            new Game(893, "黑暗侦探"),
            new Game(894, "锈湖根源"),
            new Game(895, "汽车修理工模拟2018"),
            new Game(896, "格莉斯的旅程"),
            new Game(897, "逆转裁判123成步堂精选集"),
            new Game(899, "面包房模拟器"),
            new Game(900, "尼克全明星大乱斗"),
            new Game(901, "酒店建筑师"),
            new Game(902, "学园构想家"),
            new Game(903, "后室：一起逃离"),
            new Game(904, "苏丹的游戏"),
            new Game(905, "三国群英传3"),
            new Game(906, "反恐精英"),
            new Game(907, "反恐精英：零点行动"),
            new Game(909, "血染小镇"),
            new Game(911, "杀戮时刻"),
            new Game(912, "庄园领主"),
            new Game(913, "战国王朝"),
            new Game(914, "阿加斯巴之塔"),
            new Game(915, "无主之地年度版"),
            new Game(916, "无主之地前传"),
            new Game(917, "无主之地传说"),
            new Game(918, "无主之地年度增强版"),
            new Game(919, "《新无主之地传说》"),
            new Game(920, "渎神"),
            new Game(921, "光明记忆：无限"),
            new Game(923, "侠盗猎车手圣安地列斯"),
            new Game(924, "侠盗猎车手圣安地列斯"),
            new Game(925, "乐高哈利波特：1-4年"),
            new Game(926, "乐高哈利波特：5-7年"),
            new Game(927, "乐高魔戒"),
            new Game(928, "神力科莎"),
            new Game(929, "瘟疫公司进化"),
            new Game(931, "影子战术将军之刃"),
            new Game(932, "鸡鸡的战争"),
            new Game(933, "连环清洁工"),
            new Game(936, "终结将至"),
            new Game(937, "赏金奇兵3"),
            new Game(938, "钢铁老鼠"),
            new Game(939, "当滑雪升降机发生了故障"),
            new Game(940, "风之旅人"),
            new Game(943, "锈湖天堂岛"),
            new Game(945, "天命奇御"),
            new Game(947, "氙气赛车"),
            new Game(950, "真‧三國無雙６ with 猛將傳 DX"),
            new Game(951, "冒险公社"),
            new Game(954, "Shadow Tactics: 爱子的选择 Aiko's Choice"),
            new Game(955, "说书人"),
            new Game(956, "往左一点"),
            new Game(957, "飞越13号房 完整版"),
            new Game(959, "封锁协议"),
            new Game(960, "沙丘觉醒"),
            new Game(961, "头条猎手"),
            new Game(962, "男娘恋爱物语"),
            new Game(964, "微软飞行模拟2024"),
            new Game(966, "深入后室"),
            new Game(967, "僵尸部队三部曲"),
            new Game(968, "奇异小队"),
            new Game(970, "黑客网络"),
            new Game(971, "辐射4"),
            new Game(972, "德军总部2新巨像"),
            new Game(973, "羞辱：界外魔之死"),
            new Game(974, "小王子的星球"),
            new Game(975, "欺骗公司"),
            new Game(976, "火影忍者 终极风暴羁绊"),
            new Game(977, "人中之龙４　继承传说者"),
            new Game(978, "上古之魂"),
            new Game(979, "盖房模拟器"),
            new Game(980, "模拟农场22"),
            new Game(981, "极速骑行4"),
            new Game(983, "荣耀战场2中世纪"),
            new Game(984, "廖添丁：绝代凶贼之末日"),
            new Game(985, "图灵完备"),
            new Game(986, "白昼梦之旅"),
            new Game(987, "卡普空街机馆"),
            new Game(989, "师父 Sifu"),
            new Game(992, "女王的游戏：盛世天下 媚娘篇"),
            new Game(993, "杀手2沉默刺客"),
            new Game(994, "杀手血钱"),
            new Game(995, "杀手赦免"),
            new Game(996, "杀手狙击挑战"),
            new Game(997, "杀手契约"),
            new Game(998, "沉没之地"),
            new Game(999, "貓娘樂園 Vol.1 ～La Soleil 開店了！～"),
            new Game(1000, "猫娘乐园Vol0"),
            new Game(1001, "巧克力与香子兰Vol2"),
            new Game(1002, "巧克力与香子兰3"),
            new Game(1003, "猫娘乐园Extra"),
            new Game(1004, "巧克力与香子兰4"),
            new Game(1005, "熔炉密林"),
            new Game(1007, "木偶团队"),
            new Game(1008, "火炬之光2"),
            new Game(1009, "恶意不息"),
            new Game(1010, "人道主义Z"),
            new Game(1011, "大多数"),
            new Game(1012, "冬日计划"),
            new Game(1013, "碧蓝幻想Relink"),
            new Game(1014, "米德加尔的部落"),
            new Game(1015, "胡闹搬家1"),
            new Game(1018, "同舟共济"),
            new Game(1019, "冲就完事模拟器"),
            new Game(1020, "当铺人生2"),
            new Game(1021, "幸运房东"),
            new Game(1022, "太荒初境"),
            new Game(1024, "双人大逃脱：How 2 Escape"),
            new Game(1025, "呆呆大乱斗"),
            new Game(1027, "双人大逃脱：失落的潜艇"),
            new Game(1028, "狂野西部：枪手"),
            new Game(1029, "吸血鬼德古拉2最终圣所"),
            new Game(1030, "霸权钢铁军团"),
            new Game(1031, "崛起罗马之子"),
            new Game(1032, "少数幸运儿"),
            new Game(1033, "泰坦之旅周年纪念版"),
            new Game(1034, "旁观者"),
            new Game(1037, "荣耀战场2"),
            new Game(1038, "暴君3k"),
            new Game(1039, "死神来了"),
            new Game(1042, "少数幸运儿原声带与数字物品捆绑包"),
            new Game(1043, "女巫与六便士 the sibyl and sixpence"),
            new Game(1044, "不予播出 ( Not For Broadcast )"),
            new Game(1045, "劫掠恐慌"),
            new Game(1046, "人间地狱"),
            new Game(1048, "动物迷城"),
            new Game(1049, "审判之眼：死神的遗言 Remastered"),
            new Game(1050, "瘟疫传说：安魂曲"),
            new Game(1051, "山门与幻境"),
            new Game(1052, "生化危机9：安魂曲"),
            new Game(1053, "古剑奇谭三(Gujian3)"),
            new Game(1054, "侠之道(PathOfWuxia)"),
            new Game(1055, "忍者龟：施莱德的复仇"),
            new Game(1056, "战争交响曲：尼菲林传奇"),
            new Game(1057, "神之天平"),
            new Game(1058, "逸剑风云决"),
            new Game(1060, "英雄无敌3 HD"),
            new Game(1061, "三位一体3权力圣器"),
            new Game(1062, "暗黑血统战神版"),
            new Game(1063, "暗黑血统3"),
            new Game(1064, "闪客快打单机版合集2021"),
            new Game(1065, "要塞2"),
            new Game(1066, "全面战争：战锤"),
            new Game(1067, "全面战争：战锤2"),
            new Game(1069, "通往加拿大死亡之路"),
            new Game(1070, "杀戮天使"),
            new Game(1071, "野餐大冒险"),
            new Game(1072, "被虐的诺艾尔 Complete Edition"),
            new Game(1073, "白门"),
            new Game(1074, "白影"),
            new Game(1075, "废墟图书馆"),
            new Game(1076, "方块逃脱合集"),
            new Game(1077, "杀戮的天使 Episode.Eddie"),
            new Game(1078, "内在昔日"),
            new Game(1079, "地铁繁花"),
            new Game(1080, "萌猫公园2"),
            new Game(1081, "黑之魂"),
            new Game(1082, "忍者印记"),
            new Game(1083, "火线迈阿密"),
            new Game(1084, "叛乱"),
            new Game(1085, "破门而入"),
            new Game(1086, "惡靈進化"),
            new Game(1087, "火线迈阿密2：空号"),
            new Game(1088, "尘埃拉力赛"),
            new Game(1090, "尘埃4"),
            new Game(1091, "小小英亩 / The Little Acre"),
            new Game(1092, "手动的塞缪尔 特别版"),
            new Game(1093, "勇气与荣耀"),
            new Game(1094, "无人机竞速联盟模拟器"),
            new Game(1095, "喜剧之夜"),
            new Game(1096, "破门而入2北方特遣队"),
            new Game(1097, "起飞微型无人机"),
            new Game(1098, "无人机模拟器"),
            new Game(1101, "法外枭雄：滚石城"),
            new Game(1103, "X飞行俱乐部：FPV无人机模拟器"),
            new Game(1104, "三国群英传"),
            new Game(1105, "最远的边陲"),
            new Game(1106, "队长小翼 新秀崛起"),
            new Game(1107, "波兰球：编年史"),
            new Game(1108, "YARISUTEMESUBUTA ~ 一次性交易大师 ~"),
            new Game(1109, "相声模拟器"),
            new Game(1110, "英雄立志传：三国志"),
            new Game(1111, "架空地图模拟器"),
            new Game(1112, "生化奇兵"),
            new Game(1113, "生化奇兵2"),
            new Game(1114, "地铁2033"),
            new Game(1115, "生化奇兵 重制版"),
            new Game(1116, "生化奇兵2 重制版"),
            new Game(1119, "模拟城市4豪华版"),
            new Game(1120, "机场CEO"),
            new Game(1121, "西奥小镇"),
            new Game(1122, "铁路调度模拟器"),
            new Game(1123, "凡尔登"),
            new Game(1124, "装机模拟器"),
            new Game(1126, "黑暗地带"),
            new Game(1127, "幽浮2"),
            new Game(1130, "智代After"),
            new Game(1132, "祖先遗产"),
            new Game(1133, "星之梦HD"),
            new Game(1134, "淑女同萌！"),
            new Game(1135, "Agent A - 伪装游戏"),
            new Game(1136, "狐姬零"),
            new Game(1137, "爱上火车-Pure Station-"),
            new Game(1139, "淑女同萌！-New Division-"),
            new Game(1141, "机忆"),
            new Game(1142, "吸血鬼旋律"),
            new Game(1143, "爱上火车-Last Run!!-"),
            new Game(1144, "星空列车与白的旅行"),
            new Game(1146, "纸嫁衣"),
            new Game(1147, "我的猫娘恋人"),
            new Game(1148, "乡村狂想曲"),
            new Game(1150, "8号出口"),
            new Game(1151, "鬼泣3特别版"),
            new Game(1152, "逃亡公路冒险"),
            new Game(1153, "粘粘世界"),
            new Game(1154, "狙击手幽灵战士"),
            new Game(1155, "狙击手幽灵战士2"),
            new Game(1156, "英雄萨姆3BFE"),
            new Game(1157, "索尼克世代"),
            new Game(1158, "异形地球战区"),
            new Game(1159, "心灵杀手"),
            new Game(1160, "德波尼亚"),
            new Game(1161, "行星遭遇攻击"),
            new Game(1162, "奇迹时代3"),
            new Game(1163, "圣殿春秋"),
            new Game(1164, "异常2"),
            new Game(1165, "生命不息"),
            new Game(1166, "异形韩国"),
            new Game(1167, "异形地球战区移动战役"),
            new Game(1168, "骑士与商人"),
            new Game(1169, "杰克奥兰多导演剪辑版"),
            new Game(1170, "光暗包围战"),
            new Game(1172, "奥日与黑暗森林"),
            new Game(1173, "德波尼亚：完整旅程"),
            new Game(1175, "异形守护者"),
            new Game(1177, "燥热"),
            new Game(1179, "高速公路警察模拟器"),
            new Game(1181, "瓦尔哈拉山丘"),
            new Game(1182, "疯狂机器3 - Crazy Machines 3"),
            new Game(1185, "神盾守护者"),
            new Game(1186, "看火人"),
            new Game(1188, "疾旋战姬极速对决2"),
            new Game(1189, "扣押豪华版"),
            new Game(1190, "不确定性最后的宁静之日"),
            new Game(1191, "猫忍之心"),
            new Game(1192, "核心元素"),
            new Game(1193, "银河历险记3 (Samorost 3)"),
            new Game(1196, "10秒忍者X"),
            new Game(1198, "传说法师"),
            new Game(1200, "子弹风暴完全版"),
            new Game(1203, "汐"),
            new Game(1206, "游魂2-you're the only one-"),
            new Game(1207, "艾希"),
            new Game(1208, "我的朋友佩德罗"),
            new Game(1209, "Maid Mansion (女仆洋馆)"),
            new Game(1210, "Corona Blossom-日冕之华- vol.3   跨越星际的旅行"),
            new Game(1211, "英雄萨姆融合2017"),
            new Game(1212, "拆除与建造2018"),
            new Game(1214, "苍翼默示录神观之梦"),
            new Game(1216, "北极之刃"),
            new Game(1217, "大佬竞速 / Hotshot Racing"),
            new Game(1219, "極限凸記萌萌編年史"),
            new Game(1223, "兽娘秘境：异象残影(Mysteria~Occult Shadows~)"),
            new Game(1224, "黑暗精灵"),
            new Game(1227, "废墟王国"),
            new Game(1228, "燥热心灵控制删除"),
            new Game(1229, "赤痕夜之仪式"),
            new Game(1230, "橘子班短篇集"),
            new Game(1238, "扣押2"),
            new Game(1239, "猫忍之心PLUS那智"),
            new Game(1240, "樱之杜†净梦者"),
            new Game(1242, "茂伸奇谈"),
            new Game(1243, "KOEWOTAYORINI SP / 声之寄托SP"),
            new Game(1244, "死亡学校"),
            new Game(1245, "脑袋捕手"),
            new Game(1246, "与我恋爱的废柴小恶魔"),
            new Game(1248, "最后的恶魔"),
            new Game(1249, "命运石之门0"),
            new Game(1250, "宝藏猎人克莱儿"),
            new Game(1251, "猫忍之心2"),
            new Game(1262, "幽浮奇美拉战队"),
            new Game(1265, "共享委员长"),
            new Game(1266, "漫长等待"),
            new Game(1267, "传单爱情故事"),
            new Game(1269, "翠之海"),
            new Game(1270, "Bladed Fury｜幽林怪谈"),
            new Game(1271, "樱花MMO"),
            new Game(1272, "LOVE3 -爱立方-"),
            new Game(1273, "AI梦境档案"),
            new Game(1276, "光明记忆"),
            new Game(1279, "花"),
            new Game(1280, "樱花MMO2"),
            new Game(1281, "海绵宝宝：为比奇堡而战"),
            new Game(1282, "命运石之门：比翼恋理的爱人"),
            new Game(1283, "混沌之子"),
            new Game(1284, "军团战棋 Legion War"),
            new Game(1286, "女忍者牡丹"),
            new Game(1289, "夜下降生"),
            new Game(1291, "浴血黑帮：傀儡师"),
            new Game(1292, "打工新妻莉安娜"),
            new Game(1296, "战锤冥界：阴影尖塔"),
            new Game(1299, "格斗天使SE"),
            new Game(1301, "樱花MMO3"),
            new Game(1302, "月影魅像-解放之羽-"),
            new Game(1309, "符文女孩/Rune Girl"),
            new Game(1312, "No-brainer Heroes 挂机吧！勇者"),
            new Game(1316, "斩妖行"),
            new Game(1317, "莱莎的炼金工房 ～常暗女王与秘密藏身处～"),
            new Game(1318, "初恋日记"),
            new Game(1321, "欧克按摩店"),
            new Game(1322, "赛博飞钩"),
            new Game(1324, "炼金术士柯蕾特：精华炼成物语"),
            new Game(1326, "死亡直播间"),
            new Game(1327, "红翼"),
            new Game(1328, "WWE 2K竞技场"),
            new Game(1334, "追忆夏色年华"),
            new Game(1335, "欢迎来到♥妖魔娼馆"),
            new Game(1336, "PerfectLover 完美女友"),
            new Game(1337, "TrymenT ―献给渴望改变的你― AlphA篇"),
            new Game(1339, "死亡搁浅"),
            new Game(1340, "笼中窥梦 Moncage"),
            new Game(1341, "佣兵战歌"),
            new Game(1344, "北方之魂"),
            new Game(1345, "惩戒魅魔"),
            new Game(1351, "海沙风云 Far Away"),
            new Game(1355, "Escape Dungeon-莎莉丝 地牢脱出"),
            new Game(1358, "女王的荣耀  / Queen's Glory"),
            new Game(1360, "与魅魔莉莉姆的同居生活"),
            new Game(1361, "桌面萌娘MMD"),
            new Game(1362, "城镇叠叠乐"),
            new Game(1366, "Escape Dungeon 2-银月苍狼 苏拉尔"),
            new Game(1367, "魅魔之恋"),
            new Game(1369, "赫雷斯的角斗场"),
            new Game(1371, "夏色泡影"),
            new Game(1372, "Cat's Kiss 猫研社"),
            new Game(1373, "镜花水月"),
            new Game(1374, "惡魔少女 / Devil Girl"),
            new Game(1375, "AI爱诺-机娘育成方程式"),
            new Game(1378, "Re:LieF ～献给亲爱的你～"),
            new Game(1379, "魅魔咖啡厅"),
            new Game(1381, "夏树的监禁生活"),
            new Game(1382, "监禁女王"),
            new Game(1383, "与狐妖的同居生活"),
            new Game(1384, "仙剑奇侠传七"),
            new Game(1386, "职场狂想曲"),
            new Game(1387, "欲望理论"),
            new Game(1388, "拉娜之星"),
            new Game(1389, "股市之狼"),
            new Game(1394, "与经纪人恋爱是绝对禁止！"),
            new Game(1395, "淑女同萌！-Superior Entelecheia-"),
            new Game(1396, "近月少女的礼仪"),
            new Game(1397, "不同的冬天"),
            new Game(1399, "赫雷斯的角斗场Ⅱ"),
            new Game(1400, "病房轶事 ～只有恶质护士存在的医院～"),
            new Game(1401, "抵抗魅魔-被魔纹侵蚀的女骑士的末路"),
            new Game(1402, "啵点节奏 Bits & Bops"),
            new Game(1405, "《地平线 零之曙光™》重制版"),
            new Game(1406, "运送实验怪物的人"),
            new Game(1407, "不同的星球"),
            new Game(1408, "貓娘樂園 After"),
            new Game(1409, "传染病"),
            new Game(1410, "死在百慕大"),
            new Game(1412, "紫色晶石"),
            new Game(1413, "零之世代"),
            new Game(1415, "地球防卫军5"),
            new Game(1416, "牧场模拟器"),
            new Game(1417, "终点站：僵尸幸存者"),
            new Game(1418, "亡灵巫师"),
            new Game(1419, "深渊之影"),
            new Game(1420, "超武进化：丧尸毁灭者"),
            new Game(1421, "轩辕剑外传 穹之扉"),
            new Game(1422, "仙剑奇侠传五"),
            new Game(1423, "英雄传说：闪之轨迹II：改 –The Erebonian Civil War-"),
            new Game(1424, "英雄传说：闪之轨迹I：改 –Thors Military Academy 1204–"),
            new Game(1425, "英雄传说 零之轨迹：改"),
            new Game(1426, "英雄传说 碧之轨迹：改"),
            new Game(1427, "轩辕剑贰"),
            new Game(1428, "轩辕剑外传 枫之舞"),
            new Game(1429, "明星志愿"),
            new Game(1430, "明星志愿2"),
            new Game(1431, "明星志愿3 Deluxe"),
            new Game(1432, "轩辕剑叁外传 天之痕"),
            new Game(1433, "轩辕剑叁 云和山的彼端"),
            new Game(1434, "明星志愿 闯通关"),
            new Game(1435, "明星志愿2000"),
            new Game(1436, "轩辕剑伍 一剑凌云山海情"),
            new Game(1437, "轩辕剑外传 汉之云"),
            new Game(1438, "轩辕剑外传 云之遥"),
            new Game(1439, "魔导圣战:风色幻想"),
            new Game(1440, "风色幻想2:Alive"),
            new Game(1441, "风色幻想3:罪与罚的镇魂歌"),
            new Game(1442, "风色幻想4:圣战的终焉"),
            new Game(1443, "风色幻想5:赤月战争"),
            new Game(1444, "风色幻想6:冒险奏鸣"),
            new Game(1484, "死亡岛2"),
            new Game(1689, "失落城堡2"),
            new Game(1691, "咒语旅团"),
            new Game(1781, "杯杯倒满"),
            new Game(2266, "犯罪现场清洁工"),
            new Game(2707, "英雄连3"),
            new Game(2788, "红色沙漠豪华版DLC"),
            new Game(3092, "仁王3"),
            new Game(3502, "麻布仔大冒险"),
            new Game(4041, "犯罪模拟器"),
            new Game(4133, "收获日3"),
            new Game(4664, "穹顶突击队"),
            new Game(4666, "使命召唤4现代战争"),
            new Game(4948, "寂静岭F"),
            new Game(5811, "死亡搁浅2"),
            new Game(5824, "红色沙漠：增强版"),
            new Game(5838, "帝国时代4+苏丹崛起DLC"),
            new Game(5839, "帝国时代4+东方王朝DLC"),
            new Game(5840, "枪火重生+DLC"),
            new Game(5843, "雨中冒险2+全DLC"),
            new Game(5844, "艾尔登法环：黑夜君临+见弃空洞者DLC"),
            new Game(8594, "小小梦魇3"),
            new Game(10422, "帝国时代2+三国DLC"),
            new Game(10423, "十字军之王3+普天之下DLC"),
            new Game(10424, "都市：天际线2+终极版DLC"),
            new Game(10691, "识质存在"),
            new Game(10695, "女吊"),
            new Game(11080, "消逝的光芒：困兽"),
            new Game(13860, "方舟: 生存飞升+鲍勃DLC"),
            new Game(13863, "极限竞速：地平线6"),
            new Game(14067, "007 初露锋芒"),
            new Game(15065, "乐高蝙蝠侠：黑暗骑士之遗"),
            new Game(15076, "僵尸世界大战：劫后余生版"),
            new Game(16599, "真人快打1：终极版"),
            new Game(16600, "真人快打11+终极版DLC"),
            new Game(18963, "天国：拯救2+传奇捆绑包DLC"),
            new Game(18970, "盛世天下 女帝篇"),
            new Game(19152, "战地风云™ 6"),
            new Game(19284, "鬼武者 Way of the Sword DEMO"),
            new Game(22375, "帝国时代4+岳飞传奇DLC"),
            new Game(25857, "人间地狱：越南"),
            new Game(26027, "雾影猎人"),
            new Game(26030, "风暴怕死队"),
            new Game(28320, "《战地风云 5》"),
            new Game(28327, "生灵重塑+豪华版DLC"),
            new Game(28339, "英雄连3：最终防线"),
            new Game(28353, "致命躯壳2"),
            new Game(28482, "流放者柯南+西普塔岛DLC"),
            new Game(28593, "鬼武者 Way of the Sword"),
            new Game(28601, "NBA 2K27"),
            new Game(32045, "密室逃脱模拟器2+末日DLC"),
            new Game(32053, "小小梦魇3+豪华版DLC"),
            new Game(32057, "匹诺曹的谎言+序曲DLC"),
            new Game(32058, "潜水员戴夫+丛林DLC"),
            new Game(32059, "星际战士2+黄金版DLC"),
            new Game(32060, "星际战士2+2周年版DLC"),
            new Game(32199, "遗迹2+全DLC"),
            new Game(32201, "全面战争：三国+全DLC"),
            new Game(32328, "全面战争：战锤3+终焉之主DLC"),
            new Game(35499, "帝国时代4+北方掠夺者DLC"),
        };

        private ComboBox _cmb;
        private Button _btnRent, _btnCopy, _btnCode, _btnSave, _btnOpenSave;
        private Label _lblAccValue, _lblPwdValue, _lblCode, _lblTime, _lblCountdown, _lblStatus;
        private CheckBox _chkAuto;
        private System.Windows.Forms.Timer _timer;

        private readonly List<Game> _games = new List<Game>();
        private string _token = "";
        private int _gameId, _accountId;
        private string _curAcc = "", _curPwd = "";
        private DateTime _endAt = DateTime.MinValue;
        private DateTime _codeExpireAt = DateTime.MinValue;
        private DateTime _retryAt = DateTime.MinValue;
        private DateTime _preRenewAt = DateTime.MinValue;
        private int _retryCount;
        private bool _autoRenew, _renewing, _busy;

        internal MainForm()
        {
            BuildUi();
            LoadSession();
            LoadGames();
        }

        // ================= HTTP =================

        static MainForm()
        {
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 64;
                ServicePointManager.CheckCertificateRevocationList = false;
            }
            catch { }
        }

        private static HttpClient NewClient()
        {
            HttpClient h = new HttpClient();
            h.Timeout = TimeSpan.FromSeconds(120);
            h.DefaultRequestHeaders.Add("X-Client-Source", "pc");
            h.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ZuhaomeFree/1.0");
            return h;
        }

        private static Resp Post(string path, string json, string token)
        {
            Resp res = new Resp();
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, Api + path);
                if (token != null) req.Headers.Add("Authorization", "Bearer " + token);
                req.Content = new StringContent(json == null ? "{}" : json, Encoding.UTF8, "application/json");
                HttpResponseMessage resp = Http.SendAsync(req).Result;
                res.Status = (int)resp.StatusCode;
                string text = resp.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;
                try { res.Body = ser.Deserialize<Dictionary<string, object>>(text); } catch { }
            }
            catch (Exception ex) { res.Status = -1; res.Err = ex.GetBaseException().Message; }
            return res;
        }

        private static Resp GetJson(string path, string token)
        {
            Resp res = new Resp();
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, Api + path);
                if (token != null) req.Headers.Add("Authorization", "Bearer " + token);
                HttpResponseMessage resp = Http.SendAsync(req).Result;
                res.Status = (int)resp.StatusCode;
                string text = resp.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;
                try { res.Body = ser.Deserialize<Dictionary<string, object>>(text); } catch { }
            }
            catch (Exception ex) { res.Status = -1; res.Err = ex.GetBaseException().Message; }
            return res;
        }

        private static Dictionary<string, object> Obj(Dictionary<string, object> d, string k)
        {
            if (d == null || !d.ContainsKey(k)) return null;
            return d[k] as Dictionary<string, object>;
        }

        private static string Str(Dictionary<string, object> d, string k)
        {
            if (d == null || !d.ContainsKey(k) || d[k] == null) return "";
            return Convert.ToString(d[k]);
        }

        private static int Num(Dictionary<string, object> d, string k)
        {
            if (d == null || !d.ContainsKey(k) || d[k] == null) return 0;
            int v = 0;
            int.TryParse(Convert.ToString(d[k]), out v);
            return v;
        }

        private static List<Dictionary<string, object>> Arr(Dictionary<string, object> d, string k)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            if (d == null || !d.ContainsKey(k) || d[k] == null) return res;
            System.Collections.IEnumerable e = d[k] as System.Collections.IEnumerable;
            if (e == null) return res;
            foreach (object o in e)
            {
                Dictionary<string, object> m = o as Dictionary<string, object>;
                if (m != null) res.Add(m);
            }
            return res;
        }

        private static string Short(Resp r)
        {
            if (r == null) return "无响应";
            if (r.Status == -1) return "网络异常：" + r.Err;
            if (r.Body != null)
            {
                string e = Str(r.Body, "error");
                if (e.Length == 0) e = Str(r.Body, "message");
                if (e.Length > 0) return e;
            }
            return "HTTP " + r.Status;
        }

        private static string NewUserName()
        {
            string s = "zr";
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string cs = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 6; i++) s += cs[rnd.Next(cs.Length)];
            for (int i = 0; i < 3; i++) s += rnd.Next(10);
            return s;
        }

        private static string NewPassword()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            StringBuilder sb = new StringBuilder();
            string cs = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 5; i++) sb.Append(cs[rnd.Next(cs.Length)]);
            sb.Append("xiexie");
            return sb.ToString();
        }

        private static string RentJson(int accountId, int gameId)
        {
            string Q = ((char)34).ToString();
            string s = "{";
            if (accountId > 0) s += Q + "account_id" + Q + ":" + accountId + ",";
            s += Q + "game_id" + Q + ":" + gameId;
            s += "," + Q + "duration" + Q + ":4200";
            s += "," + Q + "source" + Q + ":" + Q + "wallet" + Q + "}";
            return s;
        }        // ================= UI =================

        private void BuildUi()
        {
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            Text = "租号么 · 免费租号器";
            ClientSize = new Size(620, 604);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Microsoft YaHei UI", 9F);

            Label t = new Label();
            t.Text = "租 号 么 · 免 费 租 号 器";
            t.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold);
            t.ForeColor = Color.FromArgb(30, 41, 59);
            t.AutoSize = true;
            t.Location = new Point(22, 12);
            Controls.Add(t);

            Label sub = new Label();
            sub.Text = "选游戏 → 点按钮 → 拿 Steam 账号密码 + 验证码（70 分钟，可自动续）";
            sub.ForeColor = Color.FromArgb(100, 116, 139);
            sub.AutoSize = true;
            sub.Location = new Point(24, 56);
            Controls.Add(sub);

            Label lg = new Label();
            lg.Text = "游戏";
            lg.ForeColor = Color.FromArgb(51, 65, 85);
            lg.AutoSize = true;
            lg.Location = new Point(24, 92);
            Controls.Add(lg);

            _cmb = new ComboBox();
            _cmb.DropDownStyle = ComboBoxStyle.DropDown;
            _cmb.Font = new Font("Microsoft YaHei UI", 10.5F);
            _cmb.Location = new Point(24, 112);
            _cmb.Size = new Size(392, 28);
            _cmb.AutoCompleteMode = AutoCompleteMode.None;
            _cmb.DropDownHeight = 300;
            _cmb.MaxDropDownItems = 14;
            _cmb.TextUpdate += OnSearchText;
            _cmb.KeyDown += OnSearchKey;
            _cmb.DropDown += OnDropDown;
            _cmb.Click += OnComboClick;
            Controls.Add(_cmb);

            _btnRent = new Button();
            _btnRent.Text = "开始租号";
            _btnRent.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
            _btnRent.ForeColor = Color.White;
            _btnRent.BackColor = Color.FromArgb(37, 99, 235);
            _btnRent.FlatStyle = FlatStyle.Flat;
            _btnRent.FlatAppearance.BorderSize = 0;
            _btnRent.Location = new Point(428, 108);
            _btnRent.Size = new Size(168, 38);
            _btnRent.Cursor = Cursors.Hand;
            _btnRent.Click += OnRentClick;
            Controls.Add(_btnRent);

            GroupBox grp = new GroupBox();
            grp.Text = "  取号结果  ";
            grp.ForeColor = Color.FromArgb(100, 116, 139);
            grp.Location = new Point(20, 156);
            grp.Size = new Size(580, 284);
            Controls.Add(grp);

            Label la = new Label();
            la.Text = "Steam 账号";
            la.ForeColor = Color.FromArgb(71, 85, 105);
            la.AutoSize = true;
            la.Location = new Point(24, 32);
            grp.Controls.Add(la);

            _lblAccValue = new Label();
            _lblAccValue.Text = "——————";
            _lblAccValue.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            _lblAccValue.ForeColor = Color.FromArgb(15, 23, 42);
            _lblAccValue.AutoSize = true;
            _lblAccValue.Location = new Point(112, 27);
            grp.Controls.Add(_lblAccValue);

            Label lp = new Label();
            lp.Text = "Steam 密码";
            lp.ForeColor = Color.FromArgb(71, 85, 105);
            lp.AutoSize = true;
            lp.Location = new Point(24, 74);
            grp.Controls.Add(lp);

            _lblPwdValue = new Label();
            _lblPwdValue.Text = "——————";
            _lblPwdValue.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            _lblPwdValue.ForeColor = Color.FromArgb(15, 23, 42);
            _lblPwdValue.AutoSize = true;
            _lblPwdValue.Location = new Point(112, 69);
            grp.Controls.Add(_lblPwdValue);

            Label lc = new Label();
            lc.Text = "验证码";
            lc.ForeColor = Color.FromArgb(71, 85, 105);
            lc.AutoSize = true;
            lc.Location = new Point(24, 116);
            grp.Controls.Add(lc);

            _lblCode = new Label();
            _lblCode.Text = "—";
            _lblCode.Font = new Font("Consolas", 19F, FontStyle.Bold);
            _lblCode.ForeColor = Color.FromArgb(214, 48, 49);
            _lblCode.AutoSize = true;
            _lblCode.Location = new Point(110, 104);
            grp.Controls.Add(_lblCode);

            _btnCode = new Button();
            _btnCode.Text = "取验证码";
            _btnCode.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            _btnCode.ForeColor = Color.FromArgb(180, 83, 9);
            _btnCode.BackColor = Color.FromArgb(254, 249, 195);
            _btnCode.FlatStyle = FlatStyle.Flat;
            _btnCode.FlatAppearance.BorderColor = Color.FromArgb(250, 204, 21);
            _btnCode.Location = new Point(352, 102);
            _btnCode.Size = new Size(112, 34);
            _btnCode.Cursor = Cursors.Hand;
            _btnCode.Click += delegate { DoCode(); };
            grp.Controls.Add(_btnCode);

            _btnCopy = new Button();
            _btnCopy.Text = "复制账号密码";
            _btnCopy.ForeColor = Color.FromArgb(37, 99, 235);
            _btnCopy.BackColor = Color.White;
            _btnCopy.FlatStyle = FlatStyle.Flat;
            _btnCopy.FlatAppearance.BorderColor = Color.FromArgb(147, 197, 253);
            _btnCopy.Location = new Point(452, 24);
            _btnCopy.Size = new Size(108, 38);
            _btnCopy.Cursor = Cursors.Hand;
            _btnCopy.Click += delegate { CopyAll(); SetStatus("账号密码已复制。"); };
            grp.Controls.Add(_btnCopy);

            _lblTime = new Label();
            _lblTime.Text = "可用到：—";
            _lblTime.ForeColor = Color.FromArgb(51, 65, 85);
            _lblTime.AutoSize = true;
            _lblTime.Location = new Point(24, 158);
            grp.Controls.Add(_lblTime);

            _lblCountdown = new Label();
            _lblCountdown.Text = "—";
            _lblCountdown.ForeColor = Color.FromArgb(37, 99, 235);
            _lblCountdown.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            _lblCountdown.AutoSize = false;
            _lblCountdown.TextAlign = ContentAlignment.MiddleRight;
            _lblCountdown.Location = new Point(370, 154);
            _lblCountdown.Size = new Size(182, 24);
            grp.Controls.Add(_lblCountdown);

            Label h1 = new Label();
            h1.Text = "登 Steam 输账号密码 → 弹验证码就点【取验证码】抄进去（8~16 秒有效，过期再点）";
            h1.ForeColor = Color.FromArgb(100, 116, 139);
            h1.Font = new Font("Microsoft YaHei UI", 8.5F);
            h1.AutoSize = true;
            h1.Location = new Point(24, 196);
            grp.Controls.Add(h1);

            Label h2 = new Label();
            h2.Text = "在线玩：登录后保持在线即可联机；单机想稳就切【离线模式】，到期也不会掉线。";
            h2.ForeColor = Color.FromArgb(100, 116, 139);
            h2.Font = new Font("Microsoft YaHei UI", 8.5F);
            h2.AutoSize = true;
            h2.Location = new Point(24, 222);
            grp.Controls.Add(h2);

            _btnSave = new Button();
            _btnSave.Text = "存档接力（换号不丢档）";
            _btnSave.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            _btnSave.ForeColor = Color.White;
            _btnSave.BackColor = Color.FromArgb(22, 163, 74);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Location = new Point(20, 448);
            _btnSave.Size = new Size(210, 40);
            _btnSave.Cursor = Cursors.Hand;
            _btnSave.Click += OnSaveClick;
            Controls.Add(_btnSave);

            _btnOpenSave = new Button();
            _btnOpenSave.Text = "打开存档备份";
            _btnOpenSave.Font = new Font("Microsoft YaHei UI", 10F);
            _btnOpenSave.ForeColor = Color.FromArgb(51, 65, 85);
            _btnOpenSave.BackColor = Color.White;
            _btnOpenSave.FlatStyle = FlatStyle.Flat;
            _btnOpenSave.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            _btnOpenSave.Location = new Point(240, 448);
            _btnOpenSave.Size = new Size(150, 40);
            _btnOpenSave.Cursor = Cursors.Hand;
            _btnOpenSave.Click += delegate { OpenSaveDir(); };
            Controls.Add(_btnOpenSave);

            _chkAuto = new CheckBox();
            _chkAuto.Text = "到期自动续同一个号";
            _chkAuto.ForeColor = Color.FromArgb(51, 65, 85);
            _chkAuto.AutoSize = true;
            _chkAuto.Checked = true;
            _chkAuto.Location = new Point(404, 458);
            _chkAuto.CheckedChanged += delegate { _autoRenew = _chkAuto.Checked; };
            Controls.Add(_chkAuto);
            _autoRenew = true;

            _lblStatus = new Label();
            _lblStatus.Text = "就绪。选游戏点【开始租号】拿号；到期会自动续，不用管。";
            _lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
            _lblStatus.Location = new Point(22, 498);
            _lblStatus.Size = new Size(580, 44);
            Controls.Add(_lblStatus);

            FormClosing += delegate { SaveSession(); };
        }

        private void SetStatus(string s)
        {
            if (_lblStatus != null) _lblStatus.Text = s;
        }

        private void CopyText(string s)
        {
            try { if (!string.IsNullOrEmpty(s)) Clipboard.SetText(s); } catch { }
        }

        private void CopyAll()
        {
            if (_curAcc.Length == 0) { SetStatus("还没有号，先点【开始租号】。"); return; }
            CopyText(_curAcc + "\t" + _curPwd);
        }        // ================= 租号 =================

        private async void OnRentClick(object sender, EventArgs e)
        {
            if (_busy) return;
            if (_renewing) { SetStatus("正在自动续租，等它跑完再来（就几秒）。"); return; }
            string kw = (_cmb.Text == null) ? "" : _cmb.Text.Trim();
            if (kw.Length == 0) { SetStatus("先选一个游戏，或者直接把游戏名打进去。"); return; }

            Game g = await ResolveAsync(kw);
            if (g == null) return;

            _busy = true;
            _btnRent.Enabled = false;
            _btnRent.Text = "取号中…";
            SetStatus("正在免费注册小号 → 领签到时长 → 取号，约 3~5 秒…");

            RentResult r;
            try { r = await Task.Run(delegate { return DoRent(g); }); }
            catch (Exception ex) { r = new RentResult(); r.Error = "网络异常：" + ex.Message; }

            _btnRent.Enabled = true;
            _btnRent.Text = "开始租号";
            _busy = false;

            if (!r.Ok) { SetStatus("失败：" + r.Error); return; }
            _gameId = g.Id;
            ApplyRent(g.Id, r);
            SaveHistory(r);
            SetStatus("成功！账号密码已复制到剪贴板。弹验证码就点【取验证码】。");
        }

        private static RentResult DoRent(Game g)
        {
            RentResult r = new RentResult();
            if (g == null) { r.Error = "没选游戏"; return r; }
            string Q = ((char)34).ToString();
            string u = NewUserName();
            string pw = NewPassword();
            string body = "{" + Q + "username" + Q + ":" + Q + u + Q + "," + Q + "password" + Q + ":" + Q + pw + Q + "}";
            Resp reg = Post("/auth/register", body, null);
            if (reg.Status != 200) { r.Error = "注册失败 " + Short(reg); return r; }
            Resp login = Post("/auth/login", body, null);
            if (login.Status != 200) { r.Error = "登录失败 " + Short(login); return r; }
            string tok = Str(Obj(login.Body, "data"), "access_token");
            if (tok.Length == 0) { r.Error = "没拿到 token"; return r; }
            r.Token = tok;
            Resp claim = Post("/tasks/4/claim", "{}", tok);
            if (claim.Status != 200) { r.Error = "领时长失败 " + Short(claim); return r; }
            Resp rent = Post("/rent", RentJson(0, g.Id), tok);
            if (rent.Status != 200) { r.Error = Short(rent); return r; }
            Dictionary<string, object> d = Obj(rent.Body, "data");
            r.Acc = Str(d, "account_name");
            r.Pwd = Str(d, "account_password");
            r.EndTime = Str(d, "end_time");
            r.AccountId = Num(d, "account_id");
            r.Ok = r.Acc.Length > 0;
            if (!r.Ok) r.Error = "返回里没有账号信息";
            return r;
        }

        private Task<Game> ResolveAsync(string kw)
        {
            TaskCompletionSource<Game> tcs = new TaskCompletionSource<Game>();
            try { tcs.SetResult(ResolveLocal(kw)); }
            catch (Exception ex) { tcs.SetException(ex); }
            return tcs.Task;
        }

        private Game ResolveLocal(string kw)
        {
            string k = (kw == null) ? "" : kw.Trim().ToLower();
            if (k.Length == 0) return null;
            Game exact = null, prefix = null, contains = null;
            foreach (Game g in _games)
            {
                string n = g.Name.ToLower();
                if (n == k) { exact = g; break; }
                if (prefix == null && n.StartsWith(k)) prefix = g;
                if (contains == null && n.IndexOf(k) >= 0) contains = g;
            }
            if (exact != null) return exact;
            if (prefix != null) return prefix;
            if (contains != null) return contains;

            if (_syncState == 1) SetStatus("本地没有「" + kw.Trim() + "」。全站列表还在同步（约 1 分钟），同步完再搜。");
            else SetStatus("全站没有叫「" + kw.Trim() + "」的游戏。下拉开头打两个字，列表会实时出来。");
            return null;
        }

        private static List<Dictionary<string, object>> ToList(object raw)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            System.Collections.IEnumerable e = raw as System.Collections.IEnumerable;
            if (e == null) return res;
            foreach (object o in e)
            {
                Dictionary<string, object> m = o as Dictionary<string, object>;
                if (m != null) res.Add(m);
            }
            return res;
        }

        private static List<Game> ParseGames(Resp r)
        {
            List<Game> res = new List<Game>();
            if (r == null || r.Status != 200 || r.Body == null) return res;
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            if (r.Body.ContainsKey("data")) list = ToList(r.Body["data"]);
            if (list.Count == 0) list = Arr(r.Body, "list");
            foreach (Dictionary<string, object> m in list)
            {
                int id = Num(m, "ID");
                if (id == 0) id = Num(m, "id");
                string nm = Str(m, "Name");
                if (nm.Length == 0) nm = Str(m, "name");
                if (id > 0 && nm.Length > 0) res.Add(new Game(id, nm));
            }
            return res;
        }

        // 26MB 全量 JSON 用序列化器解析太慢，直接扫字符串，快几十倍
        private static List<Game> ParseGamesFast(string text)
        {
            List<Game> res = new List<Game>(26000);
            if (text == null || text.Length == 0) return res;
            int pos = 0;
            while (true)
            {
                int a = text.IndexOf("\"ID\":", pos, StringComparison.Ordinal);
                if (a < 0) break;
                int ns = a + 5;
                int ne = ns;
                while (ne < text.Length && char.IsDigit(text[ne])) ne++;
                int id = 0;
                if (ne > ns) int.TryParse(text.Substring(ns, ne - ns), out id);
                int b = text.IndexOf("\"Name\":\"", ne, StringComparison.Ordinal);
                if (b < 0) break;
                int k = b + 8;
                StringBuilder sb = new StringBuilder(48);
                while (k < text.Length)
                {
                    char ch = text[k];
                    if (ch == (char)92)
                    {
                        char nx = (k + 1 < text.Length) ? text[k + 1] : (char)32;
                        if (nx == (char)117 && k + 5 < text.Length)
                        {
                            int cp = 0;
                            try { cp = Convert.ToInt32(text.Substring(k + 2, 4), 16); } catch { cp = 32; }
                            sb.Append((char)cp);
                            k += 6;
                            continue;
                        }
                        if (nx == (char)110) { sb.Append((char)10); k += 2; continue; }
                        if (nx == (char)116) { sb.Append((char)9); k += 2; continue; }
                        if (nx == (char)114) { sb.Append((char)13); k += 2; continue; }
                        sb.Append(nx);
                        k += 2;
                        continue;
                    }
                    if (ch == (char)34) break;
                    sb.Append(ch);
                    k++;
                }
                string nm = sb.ToString();
                if (id > 0 && nm.Length > 0) res.Add(new Game(id, nm));
                pos = k + 1;
                if (res.Count >= 40000) break;
            }
            return res;
        }


        private int _syncState;
        private System.Windows.Forms.Timer _searchTimer;
        private string _lastFilter = "";
        private bool _suppressFilter;
        private static readonly string[] TopNames = new string[]
        {
            "黑神话：悟空", "黑神话：悟空 豪华版", "赛博朋克 2077", "赛博朋克2077+往日之影DLC", "艾尔登法环",
            "艾尔登法环+黄金树幽影DLC", "艾尔登法环：黑夜君临", "只狼：影逝二度", "双人成行", "极限竞速：地平线5",
            "怪物猎人：荒野", "博德之门3", "幻兽帕鲁", "战神5：诸神黄昏", "荒野大镖客2", "GTA5", "侠盗猎车手5",
            "红色沙漠豪华版DLC", "剑星", "明末：渊虚之羽", "最终幻想7：重生", "最终幻想16", "死亡搁浅",
            "生化危机4：重制版", "巫师3：狂猎", "消逝的光芒2", "使命召唤：现代战争3", "使命召唤4", "战地1",
            "遗迹2", "匹诺曹的谎言", "卧龙：苍天陨落", "对马岛之魂", "蜘蛛侠：迈尔斯", "女神异闻录5",
            "如龙8", "龙之信条2", "恶意不息", "城市：天际线2", "微软飞行模拟", "极品飞车：不羁",
            "地铁：离去", "深海迷航", "无人深空", "星露谷物语", "空洞骑士", "哈迪斯", "潜水员戴夫",
            "恐鬼症", "森林之子", "我的世界", "泰拉瑞亚", "光环：无限", "地平线：零之曙光", "战神4",
            "最后生还者", "全面战争：三国", "文明6", "十字军之王3", "欧洲卡车模拟2", "美国卡车模拟",
            "死亡细胞", "师父", "NBA 2K25", "EA SPORTS FC 25", "街头霸王6", "铁拳8", "拳皇15",
            "马里奥赛车8", "超级马力欧：奥德赛", "塞尔达传说：王国之泪", "精灵宝可梦", "生化危机8", "逃生3",
            "光子", "雾锁王国", "帝国时代4", "帝国时代4+岳飞传奇DLC", "星际争霸2", "红色警戒"
        };

        private void OnDropDown(object sender, EventArgs e)
        {
            // 打开下拉时全选，用户直接打字就是替换，不会接在旧名字后面
            BeginInvoke((MethodInvoker)delegate { try { if (_cmb.Focused) _cmb.SelectAll(); } catch { } });
        }

        private void OnComboClick(object sender, EventArgs e)
        {
            try { _cmb.SelectAll(); } catch { }
        }

        private void LoadGames()
        {
            foreach (Game g in Hot) _games.Add(g);
            _cmb.Text = "";
            _lastFilter = ((char)1).ToString();
            FilterList("");

            if (TryLoadCache())
            {
                _syncState = 2;
                SetStatus("就绪。" + _games.Count + " 款游戏，打名字就能搜。选好点【开始租号】。");
            }
            else
            {
                _syncState = 1;
                SetStatus("正在同步全站游戏列表（首次约 1 分钟，之后秒开）…先内置 " + _games.Count + " 款，可直接打名字试。");
                StartSync();
            }
        }

        private void FillItems(List<Game> src)
        {
            _suppressFilter = true;
            _cmb.BeginUpdate();
            _cmb.Items.Clear();
            int n = 0;
            foreach (Game g in src)
            {
                _cmb.Items.Add(g.Name);
                n++;
                if (n >= 2000) break;
            }
            _cmb.EndUpdate();
            _suppressFilter = false;
        }

        private static string ExeDir()
        {
            try
            {
                string p = Path.GetDirectoryName(Application.ExecutablePath);
                return p == null ? "" : p;
            }
            catch { return ""; }
        }

        private static List<Game> ReadTsv(string p)
        {
            List<Game> res = new List<Game>();
            string[] lines = File.ReadAllLines(p, Encoding.UTF8);
            foreach (string ln in lines)
            {
                if (ln == null || ln.Length == 0) continue;
                int tab = ln.IndexOf((char)9);
                if (tab <= 0) continue;
                int id = 0;
                int.TryParse(ln.Substring(0, tab), out id);
                string nm = ln.Substring(tab + 1).Trim();
                if (id > 0 && nm.Length > 0) res.Add(new Game(id, nm));
            }
            return res;
        }

        private bool TryLoadCache()
        {
            string dir = ExeDir();
            string[] paths = new string[]
            {
                Path.Combine(AppDir(), "games.tsv"),
                Path.Combine(AppDir(), "games.cache"),
                dir.Length > 0 ? Path.Combine(dir, "games.tsv") : "",
                dir.Length > 0 ? Path.Combine(dir, "..\\数据\\games.tsv") : ""
            };
            foreach (string p in paths)
            {
                if (p.Length == 0) continue;
                try
                {
                    if (!File.Exists(p)) continue;
                    List<Game> list = ReadTsv(p);
                    if (list.Count < 100) continue;
                    _games.Clear();
                    _games.AddRange(list);
                    _lastFilter = ((char)1).ToString();
                    FilterList("");
                    return true;
                }
                catch { }
            }
            return false;
        }

        private static string GetText(string path)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, Api + path);
                HttpResponseMessage resp = Http.SendAsync(req).Result;
                if ((int)resp.StatusCode != 200) return "";
                return resp.Content.ReadAsStringAsync().Result;
            }
            catch { return ""; }
        }

        private void StartSync()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                List<Game> list = null;
                try
                {
                    string text = GetText("/games");
                    if (text.Length > 1000) list = ParseGamesFast(text);
                    if (list != null && list.Count > 100)
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder(list.Count * 24);
                            foreach (Game g in list) sb.Append(g.Id).Append((char)9).Append(g.Name).Append((char)13).Append((char)10);
                            File.WriteAllText(Path.Combine(AppDir(), "games.tsv"), sb.ToString(), Encoding.UTF8);
                        }
                        catch { }
                    }
                }
                catch { }

                try
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        if (list == null || list.Count < 100)
                        {
                            _syncState = 0;
                            SetStatus("同步失败（网络或服务端太慢）。先内置 " + _games.Count + " 款，打名字试试；重启会再试。");
                            return;
                        }
                        string cur = _cmb.Text;
                        _games.Clear();
                        _games.AddRange(list);
                        _lastFilter = "";
                        FilterList(cur);
                        _syncState = 2;
                        SetStatus("同步完成，共 " + _games.Count + " 款游戏。打名字就能搜，回车直接租。");
                    });
                }
                catch { }
            });
        }

        private void OnSearchText(object sender, EventArgs e)
        {
            if (_suppressFilter) return;
            if (_searchTimer == null)
            {
                _searchTimer = new System.Windows.Forms.Timer();
                _searchTimer.Interval = 220;
                _searchTimer.Tick += delegate { _searchTimer.Stop(); FilterList(_cmb.Text); };
            }
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void OnSearchKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (_searchTimer != null) _searchTimer.Stop();
                FilterList(_cmb.Text);
                _cmb.DroppedDown = false;
                OnRentClick(null, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Down && !_cmb.DroppedDown && _cmb.Items.Count > 0)
            {
                _cmb.DroppedDown = true;
            }
        }

        private static int RankName(string name, string[] tokens)
        {
            string n = name.ToLower();
            if (n.Length == 0) return 9;
            if (tokens.Length > 0 && n.StartsWith(tokens[0])) return 0;
            if (tokens.Length > 0 && n.IndexOf(tokens[0]) >= 0) return 1;
            return 2;
        }

        private void FilterList(string kw)
        {
            kw = (kw == null) ? "" : kw.Trim();
            if (kw == _lastFilter) { if (kw.Length > 0 && _cmb.Focused) _cmb.DroppedDown = true; return; }
            _lastFilter = kw;
            string[] tokens = kw.ToLower().Split(new char[] { (char)32, (char)12288 }, StringSplitOptions.RemoveEmptyEntries);
            List<Game> hits = new List<Game>();
            if (tokens.Length == 0)
            {
                // 空输入：先摆精选热门，再按热度补，最后补其余
                HashSet<int> seen = new HashSet<int>();
                foreach (string tn in TopNames)
                {
                    foreach (Game g in _games)
                    {
                        if (g.Name == tn && !seen.Contains(g.Id)) { hits.Add(g); seen.Add(g.Id); break; }
                    }
                    if (hits.Count >= 60) break;
                }
                foreach (Game hg in Hot)
                {
                    foreach (Game g in _games)
                    {
                        if (g.Name == hg.Name && !seen.Contains(g.Id)) { hits.Add(g); seen.Add(g.Id); break; }
                    }
                    if (hits.Count >= 150) break;
                }
                foreach (Game g in _games)
                {
                    if (seen.Contains(g.Id)) continue;
                    hits.Add(g); seen.Add(g.Id);
                    if (hits.Count >= 220) break;
                }
            }
            else
            {
                foreach (Game g in _games)
                {
                    string low = g.Name.ToLower();
                    bool ok = true;
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        if (low.IndexOf(tokens[i]) < 0) { ok = false; break; }
                    }
                    if (ok) hits.Add(g);
                }
                hits.Sort(delegate(Game a, Game b)
                {
                    int ra = RankName(a.Name, tokens);
                    int rb = RankName(b.Name, tokens);
                    if (ra != rb) return ra.CompareTo(rb);
                    int la = a.Name.Length, lb = b.Name.Length;
                    if (la != lb) return la.CompareTo(lb);
                    return a.Id.CompareTo(b.Id);
                });
            }

            _suppressFilter = true;
            _cmb.BeginUpdate();
            _cmb.Items.Clear();
            int n = 0;
            foreach (Game g in hits) { _cmb.Items.Add(g.Name); n++; if (n >= 300) break; }
            _cmb.EndUpdate();
            _cmb.Text = kw;
            try { _cmb.SelectionStart = kw.Length; _cmb.SelectionLength = 0; } catch { }
            _suppressFilter = false;

            if (hits.Count > 0)
            {
                if (_cmb.Focused) _cmb.DroppedDown = true;
                if (tokens.Length > 0) SetStatus("「" + kw + "」匹配 " + hits.Count + " 款，下拉里挑一个，回车直接租。");
            }
            else
            {
                if (tokens.Length > 0)
                {
                    if (_syncState == 1) SetStatus("本地没搜到「" + kw + "」。全站列表还在同步（约 1 分钟），同步完再搜。");
                    else if (_syncState == 2) SetStatus("全站没有叫「" + kw + "」的游戏，换个词试试（打两个字就够）。");
                    else SetStatus("本地没搜到「" + kw + "」，换个词试试（打两个字就够）。");
                }
            }
        }

        // ================= 状态 =================

        private void ApplyRent(int gameId, RentResult r)
        {
            _gameId = gameId;
            _accountId = r.AccountId;
            _token = r.Token;
            _curAcc = r.Acc;
            _curPwd = r.Pwd;
            _lblAccValue.Text = r.Acc;
            _lblPwdValue.Text = r.Pwd;
            _lblCode.Text = "—";
            _lblCode.ForeColor = Color.FromArgb(214, 48, 49);
            _codeExpireAt = DateTime.MinValue;
            _retryAt = DateTime.MinValue;
            _preRenewAt = DateTime.MinValue;
            _retryCount = 0;
            _lblCountdown.ForeColor = Color.FromArgb(37, 99, 235);
            ParseEnd(r.EndTime);
            _lblTime.Text = "可用到：" + _endAt.ToString("yyyy-MM-dd HH:mm") + "（70 分钟）";
            CopyAll();
            SaveSession();
            StartTimer();
        }

        private void ParseEnd(string s)
        {
            DateTime e = DateTime.MinValue;
            try
            {
                DateTimeOffset o;
                if (DateTimeOffset.TryParse(s, out o)) e = o.LocalDateTime;
            }
            catch { }
            if (e == DateTime.MinValue)
            {
                DateTime p;
                if (DateTime.TryParse(s, out p)) e = p;
            }
            if (e == DateTime.MinValue) e = DateTime.Now.AddMinutes(70);
            _endAt = e;
        }

        private static string FmtLeft(TimeSpan t)
        {
            if (t.TotalSeconds <= 0) return "0分";
            int h = (int)t.TotalHours;
            int m = t.Minutes;
            if (h > 0) return h + "时" + m + "分";
            if (m > 0) return m + "分" + t.Seconds + "秒";
            return t.Seconds + "秒";
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 1000;
                _timer.Tick += TickHandler;
            }
            _timer.Start();
        }

        private void TickHandler(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (_endAt == DateTime.MinValue) return;
            bool expired = now >= _endAt;

            if (_codeExpireAt != DateTime.MinValue && now < _codeExpireAt)
            {
                int sec = (int)Math.Ceiling((_codeExpireAt - now).TotalSeconds);
                if (sec < 0) sec = 0;
                _lblCountdown.Text = "码剩 " + sec + " 秒";
                _lblCountdown.ForeColor = Color.FromArgb(214, 48, 49);
            }
            else if (_codeExpireAt != DateTime.MinValue)
            {
                _codeExpireAt = DateTime.MinValue;
                _lblCode.ForeColor = Color.FromArgb(150, 158, 170);
                _lblCountdown.ForeColor = Color.FromArgb(37, 99, 235);
                _lblCountdown.Text = expired ? "已到期" : ("还剩 " + FmtLeft(_endAt - now));
                SetStatus("验证码过期了，要输就再点一次【取验证码】。");
            }
            else
            {
                _lblCountdown.Text = expired ? "已到期" : ("还剩 " + FmtLeft(_endAt - now));
            }

            if (_busy) return;
            if (!_autoRenew || _renewing) return;
            if (_retryAt != DateTime.MinValue && now < _retryAt) return;

            if (!expired && _accountId > 0 && _preRenewAt == DateTime.MinValue && (_endAt - now).TotalSeconds <= 75)
            {
                _preRenewAt = now;
                _renewing = true;
                DoPreRenew();
                return;
            }

            if (!expired) return;
            _renewing = true;
            DoAutoRenew();
        }        // ================= 验证码 =================

        private async void DoCode()
        {
            if (_accountId <= 0) { SetStatus("先租一个号，再取验证码。"); return; }
            if (_token.Length == 0) { SetStatus("还没登录，先点【开始租号】。"); return; }
            SetStatus("正在取验证码…");
            int aid = _accountId;
            string tok = _token;
            Resp r;
            try { r = await Task.Run(delegate { return Post("/account/" + aid + "/token", "{}", tok); }); }
            catch (Exception ex) { SetStatus("取验证码出错：" + ex.Message); return; }

            if (r.Status == 200)
            {
                Dictionary<string, object> d = Obj(r.Body, "data");
                string code = Str(d, "code");
                int exp = Num(d, "expires_in");
                if (exp <= 0) exp = 16;
                if (code.Length == 0) { SetStatus("没取到码，再点一次试试。"); return; }
                _lblCode.Text = code;
                _lblCode.ForeColor = Color.FromArgb(214, 48, 49);
                _codeExpireAt = DateTime.Now.AddSeconds(exp);
                CopyText(code);
                SetStatus("验证码 " + code + " 已复制，剩 " + exp + " 秒。Steam 提示不对就等半分钟再取一次。");
            }
            else if (r.Status == 403) SetStatus("这个号已经被平台收回了，重新点【开始租号】拿一个。");
            else if (r.Status == 401) SetStatus("登录过期了，重新点【开始租号】。");
            else SetStatus("取验证码失败：" + Short(r));
        }

        // ================= 续租 =================

        private async void DoPreRenew()
        {
            int gid = _gameId;
            int aid = _accountId;
            if (aid <= 0) { _renewing = false; return; }
            SetStatus("还有 1 分多钟，正在提前抢回原号（抢到就不用重登 Steam）…");
            RentResult r;
            try { r = await Task.Run(delegate { return RenewSame(gid, aid, 16, 4000, false); }); }
            catch (Exception ex) { r = new RentResult(); r.Error = "网络异常：" + ex.Message; }
            _renewing = false;
            if (r.Ok)
            {
                _retryAt = DateTime.MinValue;
                ApplyRent(gid, r);
                SaveHistory(r);
                if (r.Pinned) SetStatus("提前续上了！还是原号 " + r.Acc + "，Steam 不用重登，接着玩。");
                else SetStatus("原号没抢到，先续了个新号 " + r.Acc + "。点【存档接力】把存档接过去。");
                return;
            }
            SetStatus("提前抢原号没成功（" + r.Error + "），到期后自动再试。");
        }

        private async void DoAutoRenew()
        {
            int gid = _gameId;
            int aid = _accountId;
            if (aid <= 0) { _renewing = false; return; }
            if (_retryCount == 0) SetStatus("到期了，正在自动续原号（Steam 不用重登），大概十几秒…");
            else SetStatus("第 " + (_retryCount + 1) + " 次尝试自动续租…");
            RentResult r;
            try { r = await Task.Run(delegate { return RenewSame(gid, aid, 6, 1500, true); }); }
            catch (Exception ex) { r = new RentResult(); r.Error = "网络异常：" + ex.Message; }
            _renewing = false;

            if (r.Ok)
            {
                _retryAt = DateTime.MinValue;
                ApplyRent(gid, r);
                SaveHistory(r);
                if (r.Pinned) SetStatus("续租成功！还是原号 " + r.Acc + "，密码没变，Steam 不用重登，直接接着玩。");
                else SetStatus("续上了新号 " + r.Acc + "（原号被别人抢了），密码已复制，Steam 换号后点【存档接力】。");
                return;
            }

            _retryCount++;
            if (_retryCount <= 15)
            {
                int wait = (_retryCount <= 3) ? 10 : ((_retryCount <= 8) ? 20 : 40);
                _retryAt = DateTime.Now.AddSeconds(wait);
                SetStatus("续租没成功（" + r.Error + "），" + wait + " 秒后自动再试第 " + _retryCount + " 次…");
            }
            else
            {
                _retryAt = DateTime.MinValue;
                SetStatus("自动续租连试 15 次都没成功：" + r.Error + " —— 多半这个游戏现在没空闲号，手动点【开始租号】或过一会儿再试。");
            }
        }

        private static RentResult RenewSame(int gameId, int accountId, int tries, int gapMs, bool fallbackNew)
        {
            RentResult r = new RentResult();
            string Q = ((char)34).ToString();
            string u = NewUserName();
            string pw = NewPassword();
            string body = "{" + Q + "username" + Q + ":" + Q + u + Q + "," + Q + "password" + Q + ":" + Q + pw + Q + "}";
            Resp reg = Post("/auth/register", body, null);
            if (reg.Status != 200) { r.Error = "注册失败 " + Short(reg); return r; }
            Resp login = Post("/auth/login", body, null);
            if (login.Status != 200) { r.Error = "登录失败 " + Short(login); return r; }
            string tok = Str(Obj(login.Body, "data"), "access_token");
            r.Token = tok;
            if (tok.Length == 0) { r.Error = "没拿到 token"; return r; }
            Resp claim = Post("/tasks/4/claim", "{}", tok);
            if (claim.Status != 200) { r.Error = "领时长失败 " + Short(claim); return r; }

            Resp rent = null;
            if (accountId > 0)
            {
                for (int i = 0; i < tries; i++)
                {
                    rent = Post("/rent", RentJson(accountId, gameId), tok);
                    if (rent.Status == 200) break;
                    if (i < tries - 1) Thread.Sleep(gapMs);
                }
            }
            if (rent == null || rent.Status != 200)
            {
                if (!fallbackNew) { r.Error = Short(rent); return r; }
                rent = Post("/rent", RentJson(0, gameId), tok);
            }
            if (rent == null || rent.Status != 200) { r.Error = Short(rent); return r; }

            Dictionary<string, object> d = Obj(rent.Body, "data");
            r.Acc = Str(d, "account_name");
            r.Pwd = Str(d, "account_password");
            r.EndTime = Str(d, "end_time");
            r.AccountId = Num(d, "account_id");
            r.Ok = r.Acc.Length > 0;
            if (!r.Ok) { r.Error = "返回里没有账号信息"; return r; }
            if (accountId > 0 && r.AccountId == accountId) r.Pinned = true;
            return r;
        }

        // ================= 存档 =================

        private void OnSaveClick(object sender, EventArgs e)
        {
            try { DoSave(); }
            catch (Exception ex) { SetStatus("存档接力出错：" + ex.Message); }
        }

        private static string AppDir()
        {
            string d = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZuhaomeFree");
            Directory.CreateDirectory(d);
            return d;
        }

        private static string SaveDir()
        {
            string d = Path.Combine(AppDir(), "saves");
            Directory.CreateDirectory(d);
            return d;
        }

        private static string SteamUserData()
        {
            try
            {
                Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
                if (k != null)
                {
                    object v = k.GetValue("SteamPath");
                    if (v != null)
                    {
                        string sp = Convert.ToString(v).Replace((char)47, (char)92);
                        string ud = Path.Combine(sp, "userdata");
                        if (Directory.Exists(ud)) return ud;
                    }
                }
            }
            catch { }
            string[] guess = new string[]
            {
                "C:\\Program Files (x86)\\Steam\\userdata",
                "C:\\Program Files\\Steam\\userdata",
                "D:\\Steam\\userdata",
                "E:\\Steam\\userdata"
            };
            foreach (string g in guess) { if (Directory.Exists(g)) return g; }
            return "";
        }

        private static void CopyDir(string src, string dst)
        {
            Directory.CreateDirectory(dst);
            foreach (string f in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
            {
                string rel = f.Substring(src.Length).TrimStart((char)92);
                string tgt = Path.Combine(dst, rel);
                string dir = Path.GetDirectoryName(tgt);
                if (dir != null && dir.Length > 0) Directory.CreateDirectory(dir);
                File.Copy(f, tgt, true);
            }
        }

        private void DoSave()
        {
            string root = SteamUserData();
            if (root.Length == 0 || !Directory.Exists(root))
            {
                SetStatus("没找到 Steam 的 userdata 目录。先登录一次 Steam，再点【存档接力】。");
                return;
            }
            string[] ids = Directory.GetDirectories(root);
            if (ids.Length == 0) { SetStatus("Steam 还没登录过任何账号，没有存档可接。"); return; }

            string bak = SaveDir();
            int saved = 0, restored = 0;

            // 1) 把每个 steamid 的 remote 存档归档到 saves\<appid>
            foreach (string id in ids)
            {
                foreach (string app in Directory.GetDirectories(id))
                {
                    string remote = Path.Combine(app, "remote");
                    if (!Directory.Exists(remote)) continue;
                    string tgt = Path.Combine(bak, Path.GetFileName(app));
                    try { CopyDir(remote, tgt); saved++; }
                    catch { }
                }
            }

            // 2) 反向：把归档补回那些还没有存档的 steamid（换号后的新号）
            foreach (string id in ids)
            {
                foreach (string app in Directory.GetDirectories(bak))
                {
                    string remote = Path.Combine(id, Path.GetFileName(app), "remote");
                    if (Directory.Exists(remote))
                    {
                        string[] has = Directory.GetFiles(remote, "*", SearchOption.AllDirectories);
                        if (has.Length > 0) continue;
                    }
                    try { CopyDir(app, remote); restored++; }
                    catch { }
                }
            }

            if (saved == 0 && restored == 0) { SetStatus("这个 Steam 账号还没有本地存档（游戏没存过档）。"); return; }
            SetStatus("存档接力完成：归档 " + saved + " 个游戏的存档，补回 " + restored + " 处。Steam 里直接读档就行。");
        }

        private void OpenSaveDir()
        {
            try { Process.Start("explorer.exe", SaveDir()); }
            catch (Exception ex) { SetStatus("打开失败：" + ex.Message); }
        }        // ================= 会话持久化 =================

        private static string SessionFile()
        {
            return Path.Combine(AppDir(), "session.txt");
        }

        private void SaveSession()
        {
            try
            {
                string s = _gameId + "\t" + _accountId + "\t" + _token + "\t"
                    + _endAt.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + _curAcc + "\t" + _curPwd;
                File.WriteAllText(SessionFile(), s, Encoding.UTF8);
            }
            catch { }
        }

        private void SaveHistory(RentResult r)
        {
            try
            {
                string f = Path.Combine(AppDir(), "history.txt");
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + _gameId + "\t"
                    + r.Acc + "\t" + r.Pwd + "\t" + r.EndTime + "\t" + r.AccountId;
                File.AppendAllText(f, line + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        private void LoadSession()
        {
            try
            {
                string f = SessionFile();
                if (!File.Exists(f)) return;
                string s = File.ReadAllText(f, Encoding.UTF8);
                s = s.TrimStart((char)0xFEFF).Trim();
                if (s.Length == 0) return;
                string[] a = s.Split((char)9);
                if (a.Length < 6) return;

                int gid = 0;
                int aid = 0;
                int.TryParse(a[0], out gid);
                int.TryParse(a[1], out aid);
                string tok = a[2];
                DateTime e;
                if (!DateTime.TryParse(a[3], out e)) return;
                string acc = a[4];
                string pwd = a[5];
                if (aid <= 0 || acc.Length == 0) return;

                if ((DateTime.Now - e).TotalHours > 2)
                {
                    SetStatus("上次的号过期很久了，点【开始租号】重新拿一个。");
                    return;
                }

                _gameId = gid;
                _accountId = aid;
                _token = tok;
                _curAcc = acc;
                _curPwd = pwd;
                _endAt = e;
                _lblAccValue.Text = acc;
                _lblPwdValue.Text = pwd;
                _lblTime.Text = "可用到：" + e.ToString("yyyy-MM-dd HH:mm") + "（上次的号，可直接取验证码）";
                bool live = e > DateTime.Now;
                _lblCountdown.Text = live ? ("还剩 " + FmtLeft(e - DateTime.Now)) : "已到期";
                StartTimer();
                if (live) SetStatus("已恢复上次的号，到期会自动续。想换游戏就重新选一个点【开始租号】。");
                else SetStatus("上次的号刚到期，正在自动续…");
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { SaveSession(); } catch { }
            base.OnFormClosing(e);
        }
    }

    internal static class Program
    {
        private static Mutex _one;

        [STAThread]
        private static void Main()
        {
            bool ok = false;
            try { _one = new Mutex(true, "ZuhaomeFree_SingleInstance", out ok); }
            catch { ok = true; }
            if (!ok)
            {
                MessageBox.Show("租号器已经在运行了，切到那个窗口就行（开两个会互相抢号）。", "租号么 · 免费租号器", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            GC.KeepAlive(_one);
        }
    }
}
