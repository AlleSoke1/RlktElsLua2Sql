using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RlktElsLuaToSql
{
    public partial class LuaToSqlForm : Form
    {

        List<LuaComponent> luaComponents = new List<LuaComponent>();

        public LuaToSqlForm()
        {
            InitializeComponent();
            InitializeApp();
            InitializeLua();
        }

        public void InitializeApp()
        {
            RlktLogger.SetLogType(RlktLogger.LogType.FORM_RICHTEXT);
            RlktLogger.SetLogOutput(richTextBox1);
        }

        #region LUA
        public void InitializeLua()
        {
            //
            LuaState.g_Lua.CreateLuaState();

            //
            BindLuaComponents();

            //Loading enum.lua
            LoadLuaScript("Enum.lua");
        }

        void BindLuaComponents()
        {
            BindLuaComponent("g_pRandomItemManager", new RandomItemManager());
        }

        void LoadLuaScript(string filename)
        {
            if (File.Exists(filename))
            {
                LuaState.g_Lua.DoFile(filename);
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = filename+"|" +filename;
                ofd.DefaultExt = "*.lua";
                ofd.Title = string.Format("Failed on finding {0}, please find it and open it.", filename);
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    LuaState.g_Lua.DoFile(ofd.FileName);
                }
            }
        }

        void BindLuaComponent(string bindName, LuaComponent luaComponent)
        {
            luaComponents.Add(luaComponent);
            LuaState.g_Lua.Bind(bindName, luaComponent);
        }
        #endregion
        #region File loading and exporting
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                //Load lua script
                LuaState.g_Lua.DoFile(ofd.FileName);

                //Print data read.
                bool bFoundAnyData = false;
                foreach(LuaComponent component in luaComponents)
                {
                    if (component.bHasData)
                    {
                        bFoundAnyData = true;
                        RlktLogger.Log(component.GetDataCount(), Color.DarkMagenta);
                    }
                }

                if(bFoundAnyData)
                    btnExport.Enabled = true;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            foreach(var component in luaComponents)
            {
                component.ExportToSql();
            }
        }
        #endregion
    }
}
