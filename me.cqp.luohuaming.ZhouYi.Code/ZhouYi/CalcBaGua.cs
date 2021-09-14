using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZhouYi;

namespace me.cqp.luohuaming.ZhouYi.Code.ZhouYi
{
    public class CalcBaGua
    {
        public static string GetGuaText()
        {
            StringBuilder sb = new StringBuilder();
            var r = DoQiGua();
            sb.AppendLine("本卦：");
            sb.AppendLine(OutBenGua(r));
            sb.AppendLine("变卦：");
            DoBianGua(ref r);
            sb.AppendLine(OutBianGua(r));
            string Gua_1 = FormatYinYang(r[0], r[1], r[2]);
            string Gua_2 = FormatYinYang(r[3], r[4], r[5]);
            sb.AppendLine("卦象：");
            sb.AppendLine($"上卦：{GuaXiang.GetXiangName(Gua_1)}");
            sb.AppendLine($"下卦：{GuaXiang.GetXiangName(Gua_2)}");
            sb.AppendLine("\n卦象：");
            sb.AppendLine($"{GuaXiang.GetGuaName(Gua_1 + Gua_2)}");
            sb.AppendLine("\n解卦：");
            sb.AppendLine(DoJieGua(r));
            return sb.ToString();
        }
        private enum GuaType
        {
            阴,
            阳
        }
        private enum GuaResult
        {
            动,
            安定
        }
        /// <summary>
        /// 起卦
        /// </summary>
        /// <returns>六次结果</returns>
        private static List<int> DoQiGua()
        {
            Random rd = new Random();
            List<int> result = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int n = 49, s = 0;
                do
                {
                    int k1 = rd.Next(2, n - 1), k2 = n - k1;
                    k1 -= 1;
                    while (k1 > 4)
                    {
                        k1 -= 4;
                    }
                    while (k2 > 4)
                    {
                        k2 -= 4;
                    }
                    n = n - k1 - k2 - 1;
                    s += 1;
                } while (s < 3);
                result.Add(n / 4);
            }
            return result;
        }
        private static string FormatYinYang(int a, int b, int c)
        {
            string res = "";
            res += GetGuaType(a) == GuaType.阳 ? 1 : 0;
            res += GetGuaType(b) == GuaType.阳 ? 1 : 0;
            res += GetGuaType(c) == GuaType.阳 ? 1 : 0;
            return res;
        }
        /// <summary>
        /// 输出本卦
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string OutBenGua(List<int> result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in result)
            {
                switch (item)
                {
                    case 6:
                        sb.AppendLine("六\t阴爻\t动爻");
                        break;
                    case 7:
                        sb.AppendLine("七\t阳爻\t定爻");
                        break;
                    case 8:
                        sb.AppendLine("八\t阴爻\t定爻");
                        break;
                    case 9:
                        sb.AppendLine("九\t阳爻\t动爻");
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 输出变卦
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string OutBianGua(List<int> result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in result)
            {
                switch (item)
                {
                    case 6:
                        sb.AppendLine("六\t阴爻\t已变卦");
                        break;
                    case 7:
                        sb.AppendLine("七\t阳爻\t定爻");
                        break;
                    case 8:
                        sb.AppendLine("八\t阴爻\t定爻");
                        break;
                    case 9:
                        sb.AppendLine("九\t阳爻\t已变卦");
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 变卦
        /// </summary>
        /// <param name="result"></param>
        private static void DoBianGua(ref List<int> result)
        {
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == 6)
                {
                    result[i] = 9;
                }
                else if (result[i] == 9)
                {
                    result[i] = 6;
                }
            }
        }
        /// <summary>
        /// 解卦
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string DoJieGua(List<int> result)
        {
            int dongCount = result.Where(x => x == 6 || x == 9).Count();
            string c = "";
            result.ForEach(x => c += GetGuaType(x) == GuaType.阳 ? 1 : 0);
            switch (dongCount)
            {
                case 0://六爻安定的，以本卦卦辞断之。
                    return GuaXiang.GetGuaCi(c + "0");
                case 1://一爻动，以动爻之爻辞断之。
                    int index = result.IndexOf(6) == -1 ? result.IndexOf(9) : result.IndexOf(6);
                    c += 6 - index;
                    return GuaXiang.GetGuaCi(c);
                case 2://两爻动者，取阴爻之爻辞断之(阳主过去，阴主未来)。所动的两爻如果同是阳爻或阴爻，则取上动之爻断之。
                    if (result.Count(x => x == 6) == 2 || result.Count(x => x == 9) == 2)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (result[i] == 6 || result[i] == 9)
                            {
                                c += 6 - i;
                                return GuaXiang.GetGuaCi(c);
                            }
                        }
                        return "";
                    }
                    else
                    {
                        c += 6 - result.IndexOf(6);
                        return GuaXiang.GetGuaCi(c);
                    }
                case 3://三爻动者，以所动三爻的中间一爻之爻辞为断之。
                    bool firstFlag = true;
                    for (int i = 0; i < 6; i++)
                    {
                        if (result[i] == 6 || result[i] == 9)
                        {
                            if (firstFlag)
                            {
                                firstFlag = false;
                                continue;
                            }
                            c += 6 - i;
                            return GuaXiang.GetGuaCi(c);
                        }
                    }
                    break;
                case 4://四爻动者，以下静之爻辞断之。
                    for (int i = 5; i >= 0; i--)
                    {
                        if (result[i] == 7 || result[i] == 8)
                        {
                            c += 6 - i;
                            return GuaXiang.GetGuaCi(c);
                        }
                    }
                    break;
                case 5://五爻动者，取静爻的爻辞断之。
                    for (int i = 0; i < 6; i++)
                    {
                        if (result[i] == 7 || result[i] == 8)
                        {
                            c += 6 - i;
                            return GuaXiang.GetGuaCi(c);
                        }
                    }
                    break;
                case 6://六爻皆动的卦，如果是乾坤二卦，以“用九”、“用六”之辞断之，乾坤两卦外其余各卦，以变卦的卦辞断之。
                    if (GuaXiang.GetGuaName(c) == "乾卦" || GuaXiang.GetGuaName(c) == "坤卦")
                    {
                        return GuaXiang.GetGuaCi(c + "6");
                    }
                    return GuaXiang.GetGuaCi(c + "0");
                default:
                    break;
            }
            return "";
        }
        private static GuaType GetGuaType(int i)
        {
            switch (i)
            {
                case 6:
                case 8:
                    return GuaType.阳;
                case 7:
                case 9:
                    return GuaType.阴;
                default:
                    return GuaType.阴;
            }
        }
        private static GuaResult GetGuaResult(int i)
        {
            switch (i)
            {
                case 6:
                case 9:
                    return GuaResult.动;
                case 7:
                case 8:
                    return GuaResult.安定;
                default:
                    return GuaResult.安定;
            }
        }

    }
}
