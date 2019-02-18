using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GView
{
    public partial class LuaPanel01
    {
        protected override Dictionary<byte, string> CreatePropertyDic()
        {
           var keywordDic = new Dictionary<byte, string>()
            {
                { luaOnInit,"on_initialize"},
                { luaUpdate,"on_update"},
                { luaOnRecover,"on_recover"},
                { luaHandleData,"on_receive"},
                { keyword_on_button_clicked,"on_button_clicked"},
            };
            return keywordDic;
        }
    }
}