using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace RlktElsLuaToSql
{
    class LuaState
    {
        //Global lua state
        public static LuaState g_Lua = new LuaState();

        public NLua.Lua m_Lua;
        

        public void CreateLuaState()
        {
            m_Lua = new NLua.Lua();
            m_Lua.State.Encoding = Encoding.UTF8;
        }

        public void DoString(string lua_code)
        {
            m_Lua.DoString(lua_code);
        }

        public void DoFile(string lua_filename)
        {
            RlktLogger.Log(string.Format("[Lua::DoFile] Loading lua script {0}", Path.GetFileName(lua_filename)));

            try
            {
                m_Lua.DoFile(lua_filename);
            }
            catch (Exception e)
            {
                RlktLogger.Log(string.Format("[Lua::DoFile] Error: {0}!", e.Message), Color.Red); //log msg error
            }
        }

        public void Bind(string name, object obj)
        {
            m_Lua[name] = obj;
        }
    }
}
