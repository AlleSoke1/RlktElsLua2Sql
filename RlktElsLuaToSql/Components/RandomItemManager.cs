using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace RlktElsLuaToSql
{
    //RandomItemTable.lua
    class RandomItemManager : LuaComponent
    {
        public RandomItemManager()
        {
            ComponentName = "RandomItemTable.lua";
            OutputTableName = "RandomItemTable";
        }


        //Random Item Data
        public Dictionary<int, List<RandomItemDrop>> randomItem = new Dictionary<int, List<RandomItemDrop>>();
        internal class RandomItemDrop
        {
            public int nItemID { get; set; }
            public float fRate { get; set; }
            public int nPeriod { get; set; }
            public int nQuantity { get; set; }

            public RandomItemDrop(int _ItemID, float _Rate, int _Period, int _Quantity)
            {
                nItemID = _ItemID;
                fRate = _Rate;
                nPeriod = _Period;
                nQuantity = _Quantity;
            }
        }

        //Random Item Templates
        public List<RandomItemTemplate> randomItemDetails = new List<RandomItemTemplate>();
        internal class RandomItemTemplate
        {
            public int nBoxItemID { get; set; }
            public int nRequiredKeyCount { get; set; }
            public int nGiveAll { get; set; }
            public int nUseCondition { get; set; }
            public int nRessCount { get; set; }
            public int nSpiritCount { get; set; }
            public int nRequiredED { get; set; }

            public List<RandomItemTemplateData> listData = new List<RandomItemTemplateData>();
        }
        internal class RandomItemTemplateData
        {
            public int nClassID { get; set; }
            public int nItemGroupID { get; set; }
            public RandomItemTemplateData(int _ClassID, int _ItemGroupID)
            {
                nClassID     = _ClassID;
                nItemGroupID = _ItemGroupID;
            }
        }

        public void AddRandomItemGroup(int nGroupID, int nItemID, float fRate, int nPeriod, int nQuantity)
        {
            if(randomItem.ContainsKey(nGroupID))
            {
                randomItem[nGroupID].Add(new RandomItemDrop(nItemID, fRate, nPeriod, nQuantity));
            }
            else
            {
                randomItem.Add(nGroupID, new List<RandomItemDrop>() { new RandomItemDrop(nItemID, fRate, nPeriod, nQuantity) } );
            }

            bHasData = true;
        }

        public void AddRandomItemTemplet(NLua.LuaTable luaTable)
        {
            RandomItemTemplate data = new RandomItemTemplate();
            
            data.nBoxItemID = Convert.ToInt32( luaTable["m_ItemID"] );
            data.nRequiredKeyCount = Convert.ToInt32( luaTable["m_RequiredKeyCount"] );
            data.nRessCount = Convert.ToInt32( luaTable["m_iRessurectionCount"] );
            data.nSpiritCount = Convert.ToInt32( luaTable["m_iRestoreSpirit"] );
            data.nRequiredED = Convert.ToInt32( luaTable["m_iRequiredED"] );
            data.nGiveAll = Convert.ToInt32( luaTable["m_bGiveAll"] );
            data.nUseCondition = Convert.ToInt32( luaTable["m_UseCondition"] );

            //Open child tables.
            foreach( object obj in luaTable.Values)
            {
                if( obj.GetType() == typeof(NLua.LuaTable) )
                {
                    NLua.LuaTable table = (NLua.LuaTable)obj;
                    int nClassID = Convert.ToInt32(table["m_cUnitClass"]);
                    int nItemGroupID = Convert.ToInt32(table["m_iItemGroupID"]);

                    data.listData.Add(new RandomItemTemplateData(nClassID, nItemGroupID));
                }
            }

            randomItemDetails.Add(data);

            bHasData = true;
        }

        public override string GetDataCount()
        {
            string tempStr = "";

            tempStr += String.Format("RandomItemGroups : {0}", randomItem.Count) + Environment.NewLine;
            tempStr += String.Format("RandomItemTemplates : {0}", randomItemDetails.Count) + Environment.NewLine;

            return tempStr;
        }

        //Create temporary data structure and save it to db.
        internal class RandomItemDBData
        {
            //Box
            public int nBoxItemID { get; set; }
            public int nRequiredKeyCount { get; set; }
            public int nGiveAll { get; set; }
            public int nUseCondition { get; set; }
            public int nRessCount { get; set; }
            public int nSpiritCount { get; set; }
            public int nRequiredED { get; set; }

            //Group class info
            public int nClassID { get; set; }
            //public int nItemGroupID { get; set; }

            //Item
            public int nItemID { get; set; }
            public float fRate { get; set; }
            public int nPeriod { get; set; }
            public int nQuantity { get; set; }
            public RandomItemDBData()
            {

            }

            public RandomItemDBData(RandomItemDBData rhs)
            {
                nBoxItemID = rhs.nBoxItemID;
                nRequiredKeyCount = rhs.nRequiredKeyCount;
                nGiveAll = rhs.nGiveAll;
                nUseCondition = rhs.nUseCondition;
                nRessCount = rhs.nRessCount;
                nSpiritCount = rhs.nSpiritCount;
                nRequiredED = rhs.nRequiredED;
                nClassID = rhs.nClassID;
                nItemID = rhs.nItemID;
                fRate = rhs.fRate;
                nPeriod = rhs.nPeriod;
                nQuantity = rhs.nQuantity;
            }
        }

        //refactor?
        public override void ExportToSql()
        {
            List<RandomItemDBData> sqlData = new List<RandomItemDBData>();

            foreach(var boxitem in randomItemDetails)
            {
                RandomItemDBData boxdetail = new RandomItemDBData();

                boxdetail.nBoxItemID = boxitem.nBoxItemID;
                boxdetail.nRequiredKeyCount = boxitem.nRequiredKeyCount;
                boxdetail.nGiveAll = boxitem.nGiveAll;
                boxdetail.nUseCondition = boxitem.nUseCondition;
                boxdetail.nRessCount = boxitem.nRessCount;
                boxdetail.nSpiritCount = boxitem.nSpiritCount;
                boxdetail.nRequiredED = boxitem.nRequiredED;

                foreach(var boxitemdropdetail in boxitem.listData)
                {
                    boxdetail.nClassID = boxitemdropdetail.nClassID;

                    if(randomItem.ContainsKey(boxitemdropdetail.nItemGroupID))
                    {
                        foreach(var item in randomItem[boxitemdropdetail.nItemGroupID])
                        {
                            boxdetail.nItemID       = item.nItemID;
                            boxdetail.fRate         = item.fRate;
                            boxdetail.nPeriod       = item.nPeriod;
                            boxdetail.nQuantity     = item.nQuantity;

                            sqlData.Add( new RandomItemDBData( boxdetail ) );
                        }
                    }
                }
            }

            SqlConnection conn = new SqlConnection("Server=10.0.0.240;Integrated Security=false;User ID=x;Password=x;DataBase=ElDataStage");
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if (conn.State == ConnectionState.Open)
            {
                //Drop the table
                try
                {
                    SqlCommand sqlCmd = new SqlCommand(string.Format("DROP TABLE {0};", OutputTableName), conn);
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                //Create table structure
                try
                {
                    SqlCommand sqlCmd = new SqlCommand( RlktSqlUtils.EnumToSql( OutputTableName, typeof(RandomItemDBData) ), conn);
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                //Insert
                foreach(var item in sqlData)
                {

                    string strQuery = string.Format("INSERT INTO {0} VALUES ({1},{2},{3},{4},{5},{6},{7},{8},{9},CAST('{10}' AS FLOAT),{11},{12})", OutputTableName,
                                       item.nBoxItemID, item.nRequiredKeyCount, item.nGiveAll, item.nUseCondition, item.nRessCount, item.nSpiritCount, item.nRequiredED, 
                                       item.nClassID, 
                                       item.nItemID, item.fRate, item.nPeriod, item.nQuantity);
                    try
                    {
                        SqlCommand sqlCmd = new SqlCommand(strQuery, conn);
                        sqlCmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }

                RlktLogger.Log(string.Format("[{0}] Exported {1} items to sql", ComponentName, sqlData.Count ));
            }
        }
    }
}
