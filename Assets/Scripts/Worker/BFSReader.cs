using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * 代码说明：Ballance专用描述文件解析类
 * 
 * 语法：(均为英文输入法的字符)
 *     属性名=属性值1;属性值2;属性值3...
 *     属性名=属性值1:属性值1子2:属性值1子3;属性值2
 *     操作(参数1,参数2,参数3...)
 *     [注释]
 *     :注释
 * 
 * 
 */

namespace Assets.Scripts.Worker
{
    /// <summary>
    /// Ballance专用描述文件解析类
    /// </summary>
    public class BFSReader : IDisposable
    {
        public BFSReader()
        {

        }
        public BFSReader(string str)
        {
            if (!string.IsNullOrEmpty(str))
                AnalysisString(str);
        }
        public BFSReader(TextAsset txt)
        {
            if (txt != null)
                if (!string.IsNullOrEmpty(txt.text))
                    AnalysisString(txt.text);
        }
        public void Dispose()
        {
            dictionaryProps.Clear();
            dictionaryProps = null;
            dictionaryActions.Clear();
            dictionaryActions = null;
        }

        private Dictionary<string, string> dictionaryProps = new Dictionary<string, string>();
        private Dictionary<string, string[]> dictionaryActions = new Dictionary<string, string[]>();

        private void AnalysisString(string str)
        {
            //按行（\n）读取
            string[] r = str.Split('\n');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] != "")
                {
                    if (!r[i].StartsWith("[") && !r[i].StartsWith(":"))
                    {
                        //解析 props
                        if (r[i].Contains("="))
                        {
                            string[] r1 = r[i].Split('=');
                            if (!dictionaryProps.ContainsKey(r1[0]))
                                dictionaryProps.Add(r1[0], r1[1]);
                        }
                        //解析 action
                        else if (r[i].Contains("(") && r[i].EndsWith(")"))
                        {
                            int iss = r[i].IndexOf('('), iee = r[i].IndexOf(')');
                            string actName = r[i].Substring(0, iss);
                            string actProp = r[i].Substring(iss + 1, iee);
                            if (actProp.Contains(";"))
                            {
                                if (!dictionaryActions.ContainsKey(actName))
                                    dictionaryActions.Add(actName, actProp.Split(';'));
                            }
                            else if (!dictionaryActions.ContainsKey(actName))
                                dictionaryActions.Add(actName, new string[] { actProp });
                        }
                        /*else if (!r[i].Contains(";"))
                        {
                            string[] r2 = r[i].Split('\n');
                            for (int i1 = 0; i1 < r2.Length; i1++)
                            {
                                if (r2[i1] != "")
                                {
                                    if (r2[i1].Contains("="))
                                    {
                                        string[] r3 = r2[i1].Split('=');
                                        if (!dictionaryProps.ContainsKey(r3[0]))
                                            dictionaryProps.Add(r3[0], r3[1]);
                                    }
                                }
                            }
                        }*/
                    }
                }
            }
        }

        /// <summary>
        /// 查询文档中是否有定义某个属性。
        /// </summary>
        /// <param name="propname">属性名字。</param>
        /// <returns></returns>
        public bool HasProperty(string propname)
        {
            return dictionaryProps.ContainsKey(propname);
        }
        /// <summary>
        /// 查询文档中是否有定义某个操作。
        /// </summary>
        /// <param name="actname">操作名字。</param>
        /// <returns></returns>
        public bool HasAction(string actname)
        {
            return dictionaryActions.ContainsKey(actname);
        }
        /// <summary>
        /// 获取某个属性的值。
        /// </summary>
        /// <param name="propname">属性名字。</param>
        /// <returns></returns>
        public string GetPropertyValue(string propname)
        {
            if (dictionaryProps.ContainsKey(propname))
                return dictionaryProps[propname];
            return null;
        }
        /// <summary>
        /// 获取属性值的分割属性。（;分隔的）
        /// </summary>
        /// <param name="propValue">属性值</param>
        /// <returns></returns>
        public string[] GetPropertyValueChildValue(string propValue)
        {
            return propValue.Split(';');
        }
        /// <summary>
        /// 获取属性值的分割属性2。（:分隔的）
        /// </summary>
        /// <param name="propValue">属性值</param>
        /// <returns></returns>
        public string[] GetPropertyValueChildValue2(string propValue)
        {
            return propValue.Split(':');
        }
        /// <summary>
        /// 获取某个操作的值。
        /// </summary>
        /// <param name="actname">操作名字。</param>
        /// <returns></returns>
        public string[] GetActionValue(string actname)
        {
            if(dictionaryActions.ContainsKey(actname))
                return dictionaryActions[actname];
            return null;
        }
    }
}
