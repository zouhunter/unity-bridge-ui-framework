#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class UnityEngineMathfWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(UnityEngine.Mathf);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 53, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Sin", _m_Sin_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Cos", _m_Cos_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Tan", _m_Tan_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Asin", _m_Asin_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Acos", _m_Acos_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Atan", _m_Atan_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Atan2", _m_Atan2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Sqrt", _m_Sqrt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Abs", _m_Abs_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Min", _m_Min_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Max", _m_Max_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Pow", _m_Pow_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Exp", _m_Exp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Log", _m_Log_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Log10", _m_Log10_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Ceil", _m_Ceil_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Floor", _m_Floor_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Round", _m_Round_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CeilToInt", _m_CeilToInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FloorToInt", _m_FloorToInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RoundToInt", _m_RoundToInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Sign", _m_Sign_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Clamp", _m_Clamp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Clamp01", _m_Clamp01_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Lerp", _m_Lerp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LerpUnclamped", _m_LerpUnclamped_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LerpAngle", _m_LerpAngle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "MoveTowards", _m_MoveTowards_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "MoveTowardsAngle", _m_MoveTowardsAngle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SmoothStep", _m_SmoothStep_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Gamma", _m_Gamma_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Approximately", _m_Approximately_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SmoothDamp", _m_SmoothDamp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SmoothDampAngle", _m_SmoothDampAngle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Repeat", _m_Repeat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PingPong", _m_PingPong_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "InverseLerp", _m_InverseLerp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClosestPowerOfTwo", _m_ClosestPowerOfTwo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GammaToLinearSpace", _m_GammaToLinearSpace_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LinearToGammaSpace", _m_LinearToGammaSpace_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsPowerOfTwo", _m_IsPowerOfTwo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "NextPowerOfTwo", _m_NextPowerOfTwo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DeltaAngle", _m_DeltaAngle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PerlinNoise", _m_PerlinNoise_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FloatToHalf", _m_FloatToHalf_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HalfToFloat", _m_HalfToFloat_xlua_st_);
            
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PI", UnityEngine.Mathf.PI);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Infinity", UnityEngine.Mathf.Infinity);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "NegativeInfinity", UnityEngine.Mathf.NegativeInfinity);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Deg2Rad", UnityEngine.Mathf.Deg2Rad);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Rad2Deg", UnityEngine.Mathf.Rad2Deg);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Epsilon", UnityEngine.Mathf.Epsilon);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				
				if (LuaAPI.lua_gettop(L) == 1)
				{
				    translator.Push(L, default(UnityEngine.Mathf));
			        return 1;
				}
				
			}
			catch(System.Exception __gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Sin_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Sin( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Cos_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Cos( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Tan_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Tan( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Asin_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Asin( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Acos_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Acos( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Atan_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Atan( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Atan2_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float y = (float)LuaAPI.lua_tonumber(L, 1);
                    float x = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Atan2( y, x );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Sqrt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Sqrt( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Abs_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Abs( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Abs( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.Abs!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Min_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Min( a, b );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int a = LuaAPI.xlua_tointeger(L, 1);
                    int b = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Min( a, b );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1))) 
                {
                    float[] values = translator.GetParams<float>(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Min( values );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1))) 
                {
                    int[] values = translator.GetParams<int>(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Min( values );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.Min!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Max_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Max( a, b );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int a = LuaAPI.xlua_tointeger(L, 1);
                    int b = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Max( a, b );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1))) 
                {
                    float[] values = translator.GetParams<float>(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Max( values );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1))) 
                {
                    int[] values = translator.GetParams<int>(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Max( values );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.Max!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Pow_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    float p = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Pow( f, p );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Exp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float power = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Exp( power );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Log( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    float p = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Log( f, p );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.Log!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log10_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Log10( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Ceil_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Ceil( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Floor_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Floor( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Round_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Round( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CeilToInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.CeilToInt( f );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FloorToInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.FloorToInt( f );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RoundToInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.RoundToInt( f );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Sign_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float f = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Sign( f );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Clamp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    float min = (float)LuaAPI.lua_tonumber(L, 2);
                    float max = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Clamp( value, min, max );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    int min = LuaAPI.xlua_tointeger(L, 2);
                    int max = LuaAPI.xlua_tointeger(L, 3);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.Clamp( value, min, max );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.Clamp!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Clamp01_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Clamp01( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Lerp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    float t = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Lerp( a, b, t );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LerpUnclamped_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    float t = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.LerpUnclamped( a, b, t );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LerpAngle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    float t = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.LerpAngle( a, b, t );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MoveTowards_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float maxDelta = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.MoveTowards( current, target, maxDelta );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MoveTowardsAngle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float maxDelta = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.MoveTowardsAngle( current, target, maxDelta );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SmoothStep_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float from = (float)LuaAPI.lua_tonumber(L, 1);
                    float to = (float)LuaAPI.lua_tonumber(L, 2);
                    float t = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothStep( from, to, t );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Gamma_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    float absmax = (float)LuaAPI.lua_tonumber(L, 2);
                    float gamma = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Gamma( value, absmax, gamma );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Approximately_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        bool __cl_gen_ret = UnityEngine.Mathf.Approximately( a, b );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SmoothDamp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 4&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDamp( current, target, ref currentVelocity, smoothTime );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 5&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    float maxSpeed = (float)LuaAPI.lua_tonumber(L, 5);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 6&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 6)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    float maxSpeed = (float)LuaAPI.lua_tonumber(L, 5);
                    float deltaTime = (float)LuaAPI.lua_tonumber(L, 6);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.SmoothDamp!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SmoothDampAngle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 4&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDampAngle( current, target, ref currentVelocity, smoothTime );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 5&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    float maxSpeed = (float)LuaAPI.lua_tonumber(L, 5);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDampAngle( current, target, ref currentVelocity, smoothTime, maxSpeed );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 6&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 6)) 
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    float currentVelocity = (float)LuaAPI.lua_tonumber(L, 3);
                    float smoothTime = (float)LuaAPI.lua_tonumber(L, 4);
                    float maxSpeed = (float)LuaAPI.lua_tonumber(L, 5);
                    float deltaTime = (float)LuaAPI.lua_tonumber(L, 6);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.SmoothDampAngle( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    LuaAPI.lua_pushnumber(L, currentVelocity);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.Mathf.SmoothDampAngle!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Repeat_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float t = (float)LuaAPI.lua_tonumber(L, 1);
                    float length = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.Repeat( t, length );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PingPong_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float t = (float)LuaAPI.lua_tonumber(L, 1);
                    float length = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.PingPong( t, length );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InverseLerp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float a = (float)LuaAPI.lua_tonumber(L, 1);
                    float b = (float)LuaAPI.lua_tonumber(L, 2);
                    float value = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.InverseLerp( a, b, value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClosestPowerOfTwo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.ClosestPowerOfTwo( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GammaToLinearSpace_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.GammaToLinearSpace( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinearToGammaSpace_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.LinearToGammaSpace( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsPowerOfTwo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = UnityEngine.Mathf.IsPowerOfTwo( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_NextPowerOfTwo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = UnityEngine.Mathf.NextPowerOfTwo( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DeltaAngle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float current = (float)LuaAPI.lua_tonumber(L, 1);
                    float target = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.DeltaAngle( current, target );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PerlinNoise_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float x = (float)LuaAPI.lua_tonumber(L, 1);
                    float y = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.PerlinNoise( x, y );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FloatToHalf_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float val = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        ushort __cl_gen_ret = UnityEngine.Mathf.FloatToHalf( val );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HalfToFloat_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    ushort val = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = UnityEngine.Mathf.HalfToFloat( val );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
