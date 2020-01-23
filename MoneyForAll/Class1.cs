using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Harmony12;
using UnityModManagerNet;
using System.Reflection;
using GameData;


namespace MoneyForAll
{
    public static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger logger;

        private static Dictionary<int, int> changedPoint = new Dictionary<int, int> {
            {61,0},/* "膂力"*/{62,0},/* "体质"*/{63,0},/* "灵敏"*/{64,0},/* "根骨"*/{65,0},/* "悟性"*/{66,0},/* "定力"*/

            {501,0},/*"音律"*/{502,0},/*"弈棋"*/{503,0},/*"诗书"*/{504,0},/*"绘画"*/{505,0},/*"术数"*/{506,0},/*"品鉴"*/{507,0},/*"锻造"*/{508,0},/*"制木"*/
            {509,0},/*"医术"*/{510,0},/*"毒术"*/{511,0},/*"织锦"*/{512,0},/*"巧匠"*/{513,0},/*"道法"*/{514,0},/*"佛学"*/{515,0},/*"厨艺"*/{516,0},/*"杂学"*/

            {601,0},/*"内功"*/{602,0},/*"身法"*/{603,0},/*"绝技"*/{604,0},/*"拳掌"*/{605,0},/*"指法"*/{606,0},/*"腿法"*/
            {607,0},/*"暗器"*/{608,0},/*"剑法"*/{609,0},/*"刀法"*/{610,0},/*"长兵"*/{611,0},/*"奇门"*/{612,0},/*"软兵"*/{613,0},/*"御射"*/{614,0},/*"乐器"*/

            {551,0},/*"技艺成长"*/{651,0},/*"功法成长"*/    //2均衡3早熟4晚成
        };
        private static Dictionary<int, string> pointDate = new Dictionary<int, string>
        {
            {61, "膂力"},{62, "体质"},{63, "灵敏"},{64, "根骨"},{65, "悟性"},{66, "定力"},

            {501,"音律"},{502,"弈棋"},{503,"诗书"},{504,"绘画"},{505,"术数"},{506,"品鉴"},{507,"锻造"},{508,"制木"},
            {509,"医术"},{510,"毒术"},{511,"织锦"},{512,"巧匠"},{513,"道法"},{514,"佛学"},{515,"厨艺"},{516,"杂学"},

            {601,"内功"},{602,"身法"},{603,"绝技"},{604,"拳掌"},{605,"指法"},{606,"腿法"},
            {607,"暗器"},{608,"剑法"},{609,"刀法"},{610,"长兵"},{611,"奇门"},{612,"软兵"},{613,"御射"},{614,"乐器"},

            {551,"技艺成长"},{651,"功法成长"},//2均衡3早熟4晚成
        };

        // 太吾余额宝
        public static bool yuEBao = true;

        // 收益率
        public static int profix = 5;

        // 太吾当铺
        public static bool dangPu = true;

        // 自定义注册事件
        private static List<int> registerEvent = new List<int>();
        public static List<int> new_event_key_list = new List<int>();

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value) return false;
            enabled = value;
            logger.Log("钱世金生MOD正在运行");
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            yuEBao = GUILayout.Toggle(yuEBao, "太吾余额宝", new GUILayoutOption[0]);
            GUILayout.Label("  年化收益率：");
            var guiProfix = GUILayout.TextField(Main.profix.ToString(), 3, GUILayout.Width(85));
            if (GUI.changed && !int.TryParse(guiProfix, out Main.profix))
            {
                Main.profix = 5;
            }
            GUILayout.Label("%");
            yuEBao = GUILayout.Toggle(yuEBao, "太吾当铺", new GUILayoutOption[0]);

        }


        /**
         * 时节变更-太吾余额宝 增加银钱
         **/
        [HarmonyPatch(typeof(UIDate), "TrunChange")]
        public static class UIDate_TrunChange_Patch
        {
            public static void Postfix()
            {
                // 时节变更结束
                // 获取太吾身上的余额
                int balance = Int32.Parse(Characters.GetCharProperty(DateFile.instance.mianActorId, 406));
                logger.Log("太吾银钱 :" + balance);
                int turnProfit = (int)(balance * 0.05 / 12);
                UIDate.instance.ChangeResource(DateFile.instance.mianActorId, 5, turnProfit, true);
                logger.Log("太吾收息银钱 :" + Characters.GetCharProperty(DateFile.instance.mianActorId, 406));
            }
        }

        /**
         * 工具方法： 新增注册事件，
         * 引用自
         * https://github.com/phorcys/Taiwu_mods/blob/master/TrainingRoom/GameCore.cs
         **/
        public static void AddNewEvent(
            int key,
            string message,
            string variables = "1",
            string requirement = "",
            string choices = "",
            string process = "",
            int nextEvent = -1,
            bool blackMask = false,
            int allBlackTime = 0,
            bool inputText = false,
            string remark = "",
            int spoker = -1,
            int backgroundID = 0)
        {
            DateFile.instance.eventDate.Add(key, new Dictionary<int, string>(){
                { 0, remark},
                { 1, backgroundID.ToString()},
                { 2, spoker.ToString()}, // 前境人物ID: -1=主角, 0=任務對象, -99=無
                { 3, message},
                { 4, variables}, //  請參考 MassageWindow.ChangeText,  
                { 5, choices},
                { 6, requirement}, // 請參考 MassageWindow.GetEventBooty
                { 7, nextEvent.ToString()},
                { 8, process},
                { 9, blackMask?"1":"0"},
                { 10, allBlackTime>0?allBlackTime.ToString():""},
                { 11, inputText ? "1" : "0"}
            });
            if (DateFile.instance.eventDate[key][5] == "") DateFile.instance.eventDate[key][5] = "0";
            registerEvent.Add(key);
            logger.Log("registerEvent Added:" + key.ToString() );
            logger.Log( DateFile.instance.eventDate[key].Select(x => String.Format("{0}:{1}", x.Key, x.Value)).ToArray().Join());
        }

        /**
         * 加载新事件
         **/
        //[HarmonyPatch(typeof(Loading), "LoadBaseDate")]
        [HarmonyPatch(typeof(MainMenu), "ShowStartGameWindow")]
        public static class Loading_LoadBaseDate_Patch
        {
            // Token: 0x0600000C RID: 12 RVA: 0x000030A0 File Offset: 0x000012A0
            public static void Postfix()
            {
                if (!Main.enabled)
                {
                    return;
                }
                Main.make_new_event_key_list();
            }
        }
        public static bool make_new_event_key_list()
        {
            logger.Log("make_new_event_key_list");
            string text = "";
            int num = DateFile.instance.eventDate.Keys.Max();
            num++;
            text = text + num.ToString() + "|";
            DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
            DateFile.instance.eventDate[num][3] = "请求提升技艺资质";
            DateFile.instance.eventDate[num][6] = "";
            DateFile.instance.eventDate[num][8] = "END&90051&893";
            Main.new_event_key_list.Add(num);
            num++;
            DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[1000000001]));
            DateFile.instance.eventDate[num][3] = "(培训班选项...)";
            DateFile.instance.eventDate[num][7] = (1 - num).ToString();
            Main.new_event_key_list.Add(num);
            foreach (int key in DateFile.instance.eventDate.Keys)
            {
                if (DateFile.instance.eventDate[key][5].Split(new char[]
                {
                    '|'
                }).Contains("1000000006"))
                {
                    DateFile.instance.eventDate[key][5] = DateFile.instance.eventDate[key][5].Replace("1000000006", num.ToString() + "|1000000006");
                }
            }
            return true;
        }
    }
}
