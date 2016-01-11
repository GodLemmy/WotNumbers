﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Code;

namespace WinApp.Code
{
	class Rating
	{
        public class RatingParameters
        {
            public RatingParameters()
            {
                BATTLES = 0;
                DAMAGE = 0;
                SPOT = 0;
                FRAGS = 0;
                DEF = 0;
                CAP = 0;
                WINS = 0;
                TIER = 0;
            }
            public double BATTLES {get; set;}
            public double DAMAGE {get; set;}
            public double SPOT {get; set;}
            public double FRAGS {get; set;}
            public double DEF {get; set;}
            public double CAP {get; set;}
            public double WINS {get; set;}
            public double TIER {get; set;}
        }

        public class RatingWN8Parameters
        {
            public RatingWN8Parameters()
            {
                rp = new RatingParameters();
                expDmg = 0;
                expSpot = 0;
                expFrag = 0;
                expDef = 0;
                expWinRate = 0;
            }
            public RatingParameters rp {get; set;}
            public double expDmg {get; set;}
            public double expSpot {get; set;}
            public double expFrag {get; set;}
            public double expDef {get; set;}
            public double expWinRate {get; set;}
        }

        private static RatingParameters GetRatingPlayerTankResults(string battleMode, bool excludeIfExpDmgIsNull = false)
        {
            RatingParameters rp = new RatingParameters();

            string battleModeWhere = "";
            string excludeIfExpDmgIsNullWhere = "";
            if (battleMode != "")
            {
                battleModeWhere = " and ptb.battleMode=@battleMode ";
                DB.AddWithValue(ref battleModeWhere, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            }
            if (excludeIfExpDmgIsNull)
            {
                excludeIfExpDmgIsNullWhere = "and t.expDmg is not null ";
            }
            string sql =
                "select sum(ptb.battles) as battles, sum(ptb.dmg) as dmg, sum (ptb.spot) as spot, sum (ptb.frags) as frags, " +
                "  sum (ptb.def) as def, sum (cap) as cap, sum(t.tier * ptb.battles) as tier, sum(ptb.wins) as wins " + 
                "from playerTankBattle ptb left join " +
                "  playerTank pt on ptb.playerTankId=pt.id left join " +
                "  tank t on pt.tankId = t.id " +
                "where pt.playerId=@playerId " + battleModeWhere + excludeIfExpDmgIsNullWhere;
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DataTable playerTotalsTable = DB.FetchData(sql);
            DataTable dt = DB.FetchData(sql);
            if (dt.Rows.Count == 0)
                return null;
            DataRow stats = dt.Rows[0];
            rp.BATTLES = Rating.ConvertDbVal2Double(stats["battles"]);
            rp.DAMAGE = Rating.ConvertDbVal2Double(stats["dmg"]);
            rp.SPOT = Rating.ConvertDbVal2Double(stats["spot"]);
            rp.FRAGS = Rating.ConvertDbVal2Double(stats["frags"]);
            rp.DEF = Rating.ConvertDbVal2Double(stats["def"]);
            rp.CAP = Rating.ConvertDbVal2Double(stats["cap"]);
            rp.WINS = Rating.ConvertDbVal2Double(stats["wins"]);
            rp.TIER = Rating.ConvertDbVal2Double(stats["tier"]);
            return rp;
        }

		#region Convert DB Value to Double

		public static double ConvertDbVal2Double(object dbValue)
		{
			double value = 0;
			if (dbValue != DBNull.Value)
			{
				value = Convert.ToDouble(dbValue);
			}
			return value;
		}

		#endregion

		#region WN8

        private static RatingWN8Parameters GetPlayerTotalWN8Params(string battleMode)
        {
            RatingWN8Parameters rpWN8 = new RatingWN8Parameters();
            // Get player totals from db
            rpWN8.rp = GetRatingPlayerTankResults(battleMode, true);
            if (rpWN8.rp == null)
                return null;
            // Get player totals
            double avgWinRate = rpWN8.rp.WINS / rpWN8.rp.BATTLES * 100;
            // Get tanks with battle count per tank and expected values from db
            string battleModeWhere = "";
            if (battleMode != "")
            {
                battleModeWhere = " and ptb.battleMode=@battleMode ";
                DB.AddWithValue(ref battleModeWhere, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            }
            string sql =
                "select sum(ptb.battles) as battles, t.id, t.expDmg, t.expSpot, t.expFrags, t.expDef, t.expWR " +
                "from playerTankBattle ptb left join " +
                "  playerTank pt on ptb.playerTankId=pt.id left join " +
                "  tank t on pt.tankId = t.id " +
                "where t.expDmg is not null and pt.playerId=@playerId " + battleModeWhere +
                "group by t.id, t.expDmg, t.expSpot, t.expFrags, t.expDef, t.expWR  ";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DataTable expectedTable = DB.FetchData(sql);
            foreach (DataRow expected in expectedTable.Rows)
            {
                // Get tanks with battle count per tank and expected values
                double battlecount = Convert.ToDouble(expected["battles"]);
                if (battlecount > 0 && expected["expDmg"] != DBNull.Value)
                {
                    rpWN8.expDmg += Convert.ToDouble(expected["expDmg"]) * battlecount;
                    rpWN8.expSpot += Convert.ToDouble(expected["expSpot"]) * battlecount;
                    rpWN8.expFrag += Convert.ToDouble(expected["expFrags"]) * battlecount;
                    rpWN8.expDef += Convert.ToDouble(expected["expDef"]) * battlecount;
                    rpWN8.expWinRate += Convert.ToDouble(expected["expWR"]) * battlecount;
                }
            }
            if (rpWN8.rp.BATTLES > 0)
                rpWN8.expWinRate = rpWN8.expWinRate / rpWN8.rp.BATTLES;
            return rpWN8;
        }

        public static double CalculatePlayerTotalWN8(string battleMode = "")
        {
            double WN8 = 0;
            RatingWN8Parameters rpWN8 = GetPlayerTotalWN8Params(battleMode);
            // Use WN8 formula to calculate result
            WN8 = UseWN8Formula(rpWN8);
            return WN8;
        }
        
        public static double CalcAvgBattleWN8(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
		{
			double WN8 = 0;
			if (battleMode == "")
				battleMode = "%";
			// Create an empty datatable with all tanks, no values
			string sql =
				"select tank.id as tankId, 0 as battles, 0 as dmg, 0 as spot, 0 as frags, " +
				"  0 as def, 0 as cap, 0 as wins " +
				"from playerTankBattle ptb left join " +
				"  playerTank pt on ptb.playerTankId=pt.id and pt.playerId=@playerId and ptb.battleMode like @battleMode left join " +
				"  tank on pt.tankId = tank.id " +
                tankJoin + " " +
				"where tank.expDmg is not null and ptb.battleMode like @battleMode " +
				"group by tank.id ";
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);

			DataTable ptb = DB.FetchData(sql);
			// Get all battles
			sql =
				"select battlesCount as battles, dmg, spotted as spot, frags, " +
				"  def, cap, tank.tier as tier , victory as wins, tank.id as tankId " +
				"from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
				"  tank on playerTank.tankId = tank.id " +
				"where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
			DataTable dtBattles = DB.FetchData(sql);
			if (dtBattles.Rows.Count > 0)
			{
				if (battleCount == 0) battleCount = dtBattles.Rows.Count;
				int count = 0;
				foreach (DataRow stats in dtBattles.Rows)
				{
					double btl = Rating.ConvertDbVal2Double(stats["battles"]);
					// add to datatable
					string tankId = stats["tankId"].ToString();
					DataRow[] ptbRow = ptb.Select("tankId = " + tankId);
					if (ptbRow.Length > 0)
					{
						ptbRow[0]["battles"] = Convert.ToInt32(ptbRow[0]["battles"]) + Convert.ToInt32(stats["battles"]);
						ptbRow[0]["dmg"] = Convert.ToInt32(ptbRow[0]["dmg"]) + Convert.ToInt32(stats["dmg"]) * btl;
						ptbRow[0]["spot"] = Convert.ToInt32(ptbRow[0]["spot"]) + Convert.ToInt32(stats["spot"]) * btl;
						ptbRow[0]["frags"] = Convert.ToInt32(ptbRow[0]["frags"]) + Convert.ToInt32(stats["frags"]) * btl;
						ptbRow[0]["def"] = Convert.ToInt32(ptbRow[0]["def"]) + Convert.ToInt32(stats["def"]) * btl;
						ptbRow[0]["cap"] = Convert.ToInt32(ptbRow[0]["cap"]) + Convert.ToInt32(stats["cap"]) * btl;
						ptbRow[0]["wins"] = Convert.ToInt32(ptbRow[0]["wins"]) + Convert.ToInt32(stats["wins"]) * btl;
					}
					else
					{
						if (Config.Settings.showDBErrors)
							Log.LogToFile("*** Could not find playerTank for battle mode '" + battleMode + "' for tank: " + tankId + " ***");
					}
					count++;
					if (count > battleCount) break;
				}
				// Check for null values
				if (ptb.Rows.Count > 0)
					WN8 = Code.Rating.CalculatePlayerTankTotalWN8(ptb);
			}
			return WN8;
		}

        public static double CalcAvgBattleWN8Reverse(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
        {
            double WN8 = 0;
            if (battleMode == "")
                battleMode = "%";
            // Create an datatable with all tanks and total stats
            string sql =
                "select tank.id as tankId, SUM(battles) as battles, SUM(dmg) as dmg, SUM(spot) as spot, SUM(frags) as frags, " +
                "  SUM(def) as def, SUM(cap) as cap, SUM(wins) as wins " +
                "from playerTankBattle ptb left join " +
                "  playerTank pt on ptb.playerTankId=pt.id and pt.playerId=@playerId and ptb.battleMode like @battleMode left join " +
                "  tank on pt.tankId = tank.id " +
                tankJoin + " " +
                "where tank.expDmg is not null and ptb.battleMode like @battleMode " +
                "group by tank.id ";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);

            DataTable ptb = DB.FetchData(sql);
            // Get all battles and subtract from totals
            sql =
                "select battlesCount as battles, dmg, spotted as spot, frags, " +
                "  def, cap, tank.tier as tier , victory as wins, tank.id as tankId " +
                "from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
                "  tank on playerTank.tankId = tank.id " +
                "where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            DataTable dtBattles = DB.FetchData(sql);
            if (dtBattles.Rows.Count > 0)
            {
                if (battleCount == 0) battleCount = dtBattles.Rows.Count;
                int count = 0;
                foreach (DataRow stats in dtBattles.Rows)
                {
                    double btl = Rating.ConvertDbVal2Double(stats["battles"]);
                    // add to datatable
                    string tankId = stats["tankId"].ToString();
                    DataRow[] ptbRow = ptb.Select("tankId = " + tankId);
                    if (ptbRow.Length > 0)
                    {
                        ptbRow[0]["battles"] = Convert.ToInt32(ptbRow[0]["battles"]) - Convert.ToInt32(stats["battles"]);
                        ptbRow[0]["dmg"] = Convert.ToInt32(ptbRow[0]["dmg"]) - Convert.ToInt32(stats["dmg"]) * btl;
                        ptbRow[0]["spot"] = Convert.ToInt32(ptbRow[0]["spot"]) - Convert.ToInt32(stats["spot"]) * btl;
                        ptbRow[0]["frags"] = Convert.ToInt32(ptbRow[0]["frags"]) - Convert.ToInt32(stats["frags"]) * btl;
                        ptbRow[0]["def"] = Convert.ToInt32(ptbRow[0]["def"]) - Convert.ToInt32(stats["def"]) * btl;
                        ptbRow[0]["cap"] = Convert.ToInt32(ptbRow[0]["cap"]) - Convert.ToInt32(stats["cap"]) * btl;
                        ptbRow[0]["wins"] = Convert.ToInt32(ptbRow[0]["wins"]) - Convert.ToInt32(stats["wins"]) * btl;
                    }
                    else
                    {
                        if (Config.Settings.showDBErrors)
                            Log.LogToFile("*** Could not find playerTank for battle mode '" + battleMode + "' for tank: " + tankId + " ***");
                    }
                    count++;
                    if (count > battleCount) break;
                }
                // Check for null values
            }
            return Code.Rating.CalculatePlayerTankTotalWN8(ptb);
        }
				
		//public static double CalculateTankWN8(int tankId, double battleCount, double dmg, double spotted, double frags, double def, double wins, bool WN8WRx = false)
        public static double CalculateTankWN8(int tankId, RatingParameters rp, bool WN8WRx = false)
		{
			Double WN8 = 0;
            RatingWN8Parameters rpWN8 = new RatingWN8Parameters();
            rpWN8.rp = rp;
			// get tankdata for current tank
			DataRow tankInfo = TankHelper.TankInfo(tankId);
			if (tankInfo != null && rp.BATTLES > 0 && tankInfo["expDmg"] != DBNull.Value)
			{
				// WN8 = Winrate for tank(s)
				double avgWinRate = rp.WINS / rp.BATTLES * 100;
				// WN8 WRx = Winrate is fixed to the expected winRate 
				if (WN8WRx)
					avgWinRate = Convert.ToDouble(tankInfo["expWR"]);
				// get wn8 exp values for tank
                rpWN8.expDmg = Convert.ToDouble(tankInfo["expDmg"]) * rp.BATTLES;
                rpWN8.expSpot = Convert.ToDouble(tankInfo["expSpot"]) * rp.BATTLES;
                rpWN8.expFrag = Convert.ToDouble(tankInfo["expFrags"]) * rp.BATTLES;
                rpWN8.expDef = Convert.ToDouble(tankInfo["expDef"]) * rp.BATTLES;
                rpWN8.expWinRate = Convert.ToDouble(tankInfo["expWR"]);
				// Use WN8 formula to calculate result
                WN8 = UseWN8Formula(rpWN8);
			}
			return WN8;
		}

		public static double CalculatePlayerTankTotalWN8(DataTable playerTankBattle)
		{
			double WN8 = 0;
			// Get player totals from datatable
			if (playerTankBattle.Rows.Count > 0)
			{
				// Get player totals
                RatingWN8Parameters rpWN8 = new RatingWN8Parameters();
                rpWN8.rp.BATTLES = Convert.ToDouble(playerTankBattle.Compute("SUM([battles])", ""));
                rpWN8.rp.DAMAGE = Convert.ToDouble(playerTankBattle.Compute("SUM([dmg])", ""));
                rpWN8.rp.SPOT = Convert.ToDouble(playerTankBattle.Compute("SUM([spot])", ""));
                rpWN8.rp.FRAGS = Convert.ToDouble(playerTankBattle.Compute("SUM([frags])", ""));
                rpWN8.rp.DEF = Convert.ToDouble(playerTankBattle.Compute("SUM([def])", ""));
                rpWN8.rp.WINS = Convert.ToDouble(playerTankBattle.Compute("SUM([wins])", ""));
                double avgWinRate = rpWN8.rp.WINS / rpWN8.rp.BATTLES * 100;
				// Get tanks with battle count per tank and expected values from db
				foreach (DataRow ptbRow in playerTankBattle.Rows)
				{
					// Get tanks with battle count per tank and expected values
					int tankId = Convert.ToInt32(ptbRow["tankId"]);
					double battlecount = Convert.ToDouble(ptbRow["battles"]);
					DataRow expected = TankHelper.TankInfo(tankId);
					if (battlecount > 0 && expected != null && expected["expDmg"] != DBNull.Value)
					{
                        rpWN8.expDmg += Convert.ToDouble(expected["expDmg"]) * battlecount;
                        rpWN8.expSpot += Convert.ToDouble(expected["expSpot"]) * battlecount;
                        rpWN8.expFrag += Convert.ToDouble(expected["expFrags"]) * battlecount;
                        rpWN8.expDef += Convert.ToDouble(expected["expDef"]) * battlecount;
                        rpWN8.expWinRate += Convert.ToDouble(expected["expWR"]) * battlecount;
					}
				}
				// Use WN8 formula to calculate result
				if (rpWN8.rp.BATTLES > 0)
                {
                    rpWN8.expWinRate = rpWN8.expWinRate / rpWN8.rp.BATTLES;
                    WN8 = UseWN8Formula(rpWN8);
                }
					
			}
			return WN8;
		}
		
		private static double UseWN8Formula(RatingWN8Parameters rpWN8)
		{
            // WN8 = Winrate for tank(s)
            double avgWinRate = rpWN8.rp.WINS / rpWN8.rp.BATTLES * 100;
            // Step 1
            double rDAMAGE = rpWN8.rp.DAMAGE / rpWN8.expDmg;
            double rSPOT = rpWN8.rp.SPOT / rpWN8.expSpot;
            double rFRAG = rpWN8.rp.FRAGS / rpWN8.expFrag;
            double rDEF = rpWN8.rp.DEF / rpWN8.expDef;
            double rWIN = avgWinRate / rpWN8.expWinRate;
			// Step 2
			double rWINc = Math.Max(0, (rWIN - 0.71) / (1 - 0.71));
			double rDAMAGEc = Math.Max(0, (rDAMAGE - 0.22) / (1 - 0.22));
			double rFRAGc = Math.Max(0, Math.Min(rDAMAGEc + 0.2, (rFRAG - 0.12) / (1 - 0.12)));
			double rSPOTc = Math.Max(0, Math.Min(rDAMAGEc + 0.1, (rSPOT - 0.38) / (1 - 0.38)));
			double rDEFc = Math.Max(0, Math.Min(rDAMAGEc + 0.1, (rDEF - 0.10) / (1 - 0.10)));
			// Step 3
			double WN8 = (980 * rDAMAGEc) + (210 * rDAMAGEc * rFRAGc) + (155 * rFRAGc * rSPOTc) + (75 * rDEFc * rFRAGc) + (145 * Math.Min(1.8, rWINc));
			// Return value
			return WN8;
		}

		public static void UseWN8FormulaReturnResult(
			RatingParameters rp, double avgWinRate, double expDmg, double expSpot, double expFrag, double expDef, double expWinRate,
			out double rWINc, out double rDAMAGEc, out double rFRAGc, out double rSPOTc, out double rDEFc)
		{
			// Step 1
			double rDAMAGE = rp.DAMAGE / expDmg;
			double rSPOT = rp.SPOT / expSpot;
			double rFRAG = rp.FRAGS / expFrag;
			double rDEF = rp.DEF / expDef;
			double rWIN = avgWinRate / expWinRate;
			// Step 2
			double rWIN2 = Math.Max(0, (rWIN - 0.71) / (1 - 0.71));
			double rDAMAGE2 = Math.Max(0, (rDAMAGE - 0.22) / (1 - 0.22));
			double rFRAG2 = Math.Max(0, Math.Min(rDAMAGE2 + 0.2, (rFRAG - 0.12) / (1 - 0.12)));
			double rSPOT2 = Math.Max(0, Math.Min(rDAMAGE2 + 0.1, (rSPOT - 0.38) / (1 - 0.38)));
			double rDEF2 = Math.Max(0, Math.Min(rDAMAGE2 + 0.1, (rDEF - 0.10) / (1 - 0.10)));
			// Step 3
			rDAMAGEc = (980 * rDAMAGE2);
			rFRAGc = (210 * rDAMAGE2 * rFRAG2);
			rSPOTc = (155 * rFRAG2 * rSPOT2);
			rDEFc = (75 * rDEF2 * rFRAG2);
			rWINc = (145 * Math.Min(1.8, rWIN2));
			
		}
		

		#endregion

		#region WN7

		public static double CalcTotalWN7(string battleMode = "15")
		{
            RatingParameters rp = GetRatingPlayerTankResults(battleMode);
            if (rp == null)
                return 0;
            double avgTier = Rating.GetAverageBattleTier(battleMode);
            return CalculateWN7(rp, avgTier);
		}

		public static double CalcBattleWN7(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
		{
			double WN7 = 0;
			if (battleMode == "")
				battleMode = "%";
			string sql =
					"select battlesCount as battles, dmg, spotted as spot, frags, " +
					"  def, cap, tank.tier as tier , victory as wins " +
					"from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
					"  tank on playerTank.tankId = tank.id " +
                    tankJoin + " " +
					"where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
			DataTable dtBattles = DB.FetchData(sql);
			if (dtBattles.Rows.Count > 0)
			{
				if (battleCount == 0) battleCount = dtBattles.Rows.Count;
				int count = 0;
                RatingParameters rp = new RatingParameters();
				foreach (DataRow stats in dtBattles.Rows)
				{
					double btl = Rating.ConvertDbVal2Double(stats["battles"]);
					rp.BATTLES += btl;
                    rp.DAMAGE += Rating.ConvertDbVal2Double(stats["dmg"]) * btl;
                    rp.SPOT += Rating.ConvertDbVal2Double(stats["spot"]) * btl;
                    rp.FRAGS += Rating.ConvertDbVal2Double(stats["frags"]) * btl;
                    rp.DEF += Rating.ConvertDbVal2Double(stats["def"]) * btl;
                    rp.CAP += Rating.ConvertDbVal2Double(stats["cap"]) * btl;
                    rp.WINS += Rating.ConvertDbVal2Double(stats["wins"]) * btl;
                    rp.TIER += Rating.ConvertDbVal2Double(stats["tier"]) * btl;
					count++;
					if (count > battleCount) break;
				}
                if (rp.BATTLES > 0)
					WN7 = Code.Rating.CalculateWN7(rp, (rp.TIER / rp.BATTLES));
			}
			return WN7;
		}

        public static double CalcBattleWN7Reverse(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
        {
            // Find current total EFF
            RatingParameters rp = GetRatingPlayerTankResults(battleMode);
            if (rp == null)
                return 0;
            double avgTier = Rating.GetAverageBattleTier(battleMode);
            double totalWN7 = CalculateWN7(rp, avgTier);

            // Find changes and subtract
            if (battleMode == "")
                battleMode = "%";
            string sql =
                    "select battlesCount as battles, dmg, spotted as spot, frags, " +
                    "  def, cap, tank.tier as tier , victory as wins " +
                    "from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
                    "  tank on playerTank.tankId = tank.id " +
                    tankJoin + " " +
                    "where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            DataTable dtBattles = DB.FetchData(sql);
            if (dtBattles.Rows.Count > 0)
            {
                if (battleCount == 0) battleCount = dtBattles.Rows.Count;
                int count = 0;
                foreach (DataRow stats in dtBattles.Rows)
                {
                    double btl = Rating.ConvertDbVal2Double(stats["battles"]);
                    rp.BATTLES -= btl;
                    rp.DAMAGE -= Rating.ConvertDbVal2Double(stats["dmg"]) * btl;
                    rp.SPOT -= Rating.ConvertDbVal2Double(stats["spot"]) * btl;
                    rp.FRAGS -= Rating.ConvertDbVal2Double(stats["frags"]) * btl;
                    rp.DEF -= Rating.ConvertDbVal2Double(stats["def"]) * btl;
                    rp.CAP -= Rating.ConvertDbVal2Double(stats["cap"]) * btl;
                    rp.WINS -= Rating.ConvertDbVal2Double(stats["wins"]) * btl;
                    rp.TIER -= Rating.ConvertDbVal2Double(stats["tier"]) * btl;
                    count++;
                    if (count > battleCount) break;
                }
            }
            return Code.Rating.CalculateWN7(rp, (rp.TIER / rp.BATTLES));
        }


		public static double CalculateWN7(RatingParameters rp, double tier, bool calcForBattle = false)
		{
			double WN7 = 0;
			if (rp.BATTLES > 0 && rp.TIER > 0)
			{
				// Calc average values
                double DAMAGE = rp.DAMAGE / rp.BATTLES;
                double SPOT = rp.SPOT / rp.BATTLES;
                double FRAGS = rp.FRAGS / rp.BATTLES;
                double DEF = rp.DEF / rp.BATTLES;
                double CAP = rp.CAP / rp.BATTLES;
                double WINRATE = rp.WINS / rp.BATTLES;
                double TIER = tier; // Override battle tier using special WN7 tier calculation
				// For battle calculations set WinRate to 50%
                if (rp.BATTLES == 1 || calcForBattle)
					WINRATE = 0.5; 
				// Calculate subvalues
				double WN7_Frags = (1240 - 1040 / Math.Pow(Math.Min(TIER, 6), 0.164)) * (FRAGS);
				double WN7_Damage = DAMAGE * 530 / (184 * Math.Exp(0.24 * TIER) + 130);
				double WN7_Spot = (SPOT) * 125 * (Math.Min(TIER, 3)) / 3;
				double WN7_Defense = Math.Min(DEF, 2.2) * 100;
				double WN7_Winrate = ((185 / (0.17 + Math.Exp(((WINRATE*100) - 35) * -0.134))) - 500) * 0.45;
                double WN7_LowTierPenalty = ((5 - Math.Min(TIER, 5)) * 125) / (1 + Math.Exp(TIER - Math.Pow(rp.BATTLES / 220, 3 / TIER)) * 1.5);
				// Find result
				WN7 = WN7_Frags + WN7_Damage + WN7_Spot + WN7_Defense + WN7_Winrate - WN7_LowTierPenalty;
			}
			// Return value
			return WN7;
		}

		#endregion

		#region EFF

		public static double CalcTotalEFF(string battleMode = "15")
		{
            RatingParameters rp = GetRatingPlayerTankResults(battleMode);
            if (rp == null)
                return 0;
            return CalculateEFF(rp);
		}

		public static double CalcBattleEFF(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
		{
			double EFF = 0;
			if (battleMode == "")
				battleMode = "%";
			string sql =
					"select battlesCount as battles, dmg, spotted as spot, frags, " +
					"  def, cap, tank.tier as tier , victory as wins " +
					"from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
					"  tank on playerTank.tankId = tank.id " +
                    tankJoin + " " +
					"where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
			DataTable dtBattles = DB.FetchData(sql);
			if (dtBattles.Rows.Count > 0)
			{
				if (battleCount == 0) battleCount = dtBattles.Rows.Count;
				int count = 0;
                RatingParameters rp = new RatingParameters();
				foreach (DataRow stats in dtBattles.Rows)
				{
					double btl = Rating.ConvertDbVal2Double(stats["battles"]);
					rp.BATTLES += btl;
					rp.DAMAGE += Rating.ConvertDbVal2Double(stats["dmg"]) * btl;
					rp.SPOT += Rating.ConvertDbVal2Double(stats["spot"]) * btl;
                    rp.FRAGS += Rating.ConvertDbVal2Double(stats["frags"]) * btl;
                    rp.DEF += Rating.ConvertDbVal2Double(stats["def"]) * btl;
                    rp.CAP += Rating.ConvertDbVal2Double(stats["cap"]) * btl;
                    rp.WINS += Rating.ConvertDbVal2Double(stats["wins"]) * btl;
                    rp.TIER += Rating.ConvertDbVal2Double(stats["tier"]) * btl;
					count++;
					if (count > battleCount) break;
				}
                if (rp.BATTLES > 0)
				{
					EFF = Code.Rating.CalculateEFF(rp);
				}
			}
			return EFF;
		}

        // Special calculation finds previous efficiency based on effiency parameters now - battle recorded parameters
        public static double CalcBattleEFFReverse(string battleTimeFilter, int battleCount = 0, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
        {
            // Find current total EFF
            RatingParameters rp = GetRatingPlayerTankResults(battleMode);
            if (rp == null)
                return 0;
            double totalEff = CalculateEFF(rp);
            
            // Find changes and subtract
            if (battleMode == "")
                battleMode = "%";
            string sql =
                    "select battlesCount as battles, dmg, spotted as spot, frags, " +
                    "  def, cap, tank.tier as tier , victory as wins " +
                    "from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
                    "  tank on playerTank.tankId = tank.id " +
                    tankJoin + " " +
                    "where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            DataTable dtBattles = DB.FetchData(sql);
            if (dtBattles.Rows.Count > 0)
            {
                if (battleCount == 0) battleCount = dtBattles.Rows.Count;
                int count = 0;
                foreach (DataRow stats in dtBattles.Rows)
                {
                    double btl = Rating.ConvertDbVal2Double(stats["battles"]);
                    rp.BATTLES -= btl;
                    rp.DAMAGE -= Rating.ConvertDbVal2Double(stats["dmg"]) * btl;
                    rp.SPOT -= Rating.ConvertDbVal2Double(stats["spot"]) * btl;
                    rp.FRAGS -= Rating.ConvertDbVal2Double(stats["frags"]) * btl;
                    rp.DEF -= Rating.ConvertDbVal2Double(stats["def"]) * btl;
                    rp.CAP -= Rating.ConvertDbVal2Double(stats["cap"]) * btl;
                    rp.WINS -= Rating.ConvertDbVal2Double(stats["wins"]) * btl;
                    rp.TIER -= Rating.ConvertDbVal2Double(stats["tier"]) * btl;
                    count++;
                    if (count > battleCount) break;
                }
            }
            return Code.Rating.CalculateEFF(rp);
        }

		public static double CalculateTankEFF(int tankId, RatingParameters rp)
		{
			// Get tankdata for current tank to get tier
			DataRow tankInfo = TankHelper.TankInfo(tankId);
			double tier = 0;
			if (tankInfo != null)
			{
				tier = Convert.ToDouble(tankInfo["tier"]);
			}
            rp.TIER = tier;
			// Call method for calc EFF
			return CalculateEFF(rp);
		}
		
		//public static double CalculateEFF(double battleCount, double dmg, double spotted, double frags, double def, double cap, double tier)
        public static double CalculateEFF(RatingParameters rp)
		{
			double EFF = 0;
			if (rp.BATTLES > 0)
			{
				double DAMAGE = rp.DAMAGE / rp.BATTLES;
                double SPOT = rp.SPOT / rp.BATTLES;
                double FRAGS = rp.FRAGS / rp.BATTLES;
                double DEF = rp.DEF / rp.BATTLES;
                double CAP = rp.CAP / rp.BATTLES;
                double TIER = rp.TIER / rp.BATTLES;
				// CALC
				EFF =
                    DAMAGE * (10 / (TIER + 2)) * (0.23 + 2 * TIER / 100) +
					FRAGS * 250 +
					SPOT * 150 +
					Math.Log(CAP + 1, 1.732) * 150 +
					DEF * 150;
			}
			// Return value
			return EFF;
		}

        public static double CalcBattleWR(string battleTimeFilter, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
        {
            double WR = 0;
            if (battleMode == "")
                battleMode = "%";
            string sql =
                "select battlesCount as battles, victory as wins " +
                "from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
                "  tank on playerTank.tankId = tank.id " +
                tankJoin + " " +
                "where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + " order by battleTime DESC";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            DataTable dtBattles = DB.FetchData(sql);
            if (dtBattles.Rows.Count > 0)
            {
                double BATTLES = 0;
                double WINS = 0;
                foreach (DataRow stats in dtBattles.Rows)
                {
                    BATTLES += Rating.ConvertDbVal2Double(stats["battles"]);
                    WINS += Rating.ConvertDbVal2Double(stats["wins"]);
                }
                if (BATTLES > 0)
                {
                    WR = Math.Round(WINS / BATTLES * 100, 2);
                }
            }
            return WR;
        }

        public static double CalcTankWR(string battleTimeFilter, string battleMode = "15", string tankFilter = "", string battleModeFilter = "", string tankJoin = "")
        {
            double WR = 0;
            if (battleMode == "")
                battleMode = "%";
            string sql = 
                "select battles, wins " +
                "from playerTankBattle " +
                "where playerTankId IN " +
                "  (select distinct playerTank.id " +
                "  from battle INNER JOIN playerTank ON battle.playerTankId=playerTank.Id left join " +
                "    tank on playerTank.tankId = tank.id " +
                "  " + tankJoin + " " +
                "  where playerId=@playerId and battleMode like @battleMode " + battleTimeFilter + " " + tankFilter + " " + battleModeFilter + ")";
            DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
            DB.AddWithValue(ref sql, "@battleMode", battleMode, DB.SqlDataType.VarChar);
            DataTable dtBattles = DB.FetchData(sql);
            if (dtBattles.Rows.Count > 0)
            {
                double BATTLES = 0;
                double WINS = 0;
                foreach (DataRow stats in dtBattles.Rows)
                {
                    BATTLES += Rating.ConvertDbVal2Double(stats["battles"]);
                    WINS += Rating.ConvertDbVal2Double(stats["wins"]);
                }
                if (BATTLES > 0)
                {
                    WR = Math.Round(WINS / BATTLES * 100, 2);
                }
            }
            return WR;
        }
        
        #endregion
					
		
		#region Recalculate Battles

		public static void RecalcBattlesWN7()
		{
			string sql = "select battle.*, playerTank.tankId as tankId from battle inner join playerTank on battle.playerTankId=playerTank.Id ORDER BY battle.id DESC";
			DataTable dtBattles = DB.FetchData(sql);
			foreach (DataRow battle in dtBattles.Rows)
			{
				// Get rating parameters
                RatingParameters rp = new RatingParameters();
				string battleId = Convert.ToInt32(battle["id"]).ToString();
				rp.DAMAGE = Rating.ConvertDbVal2Double(battle["dmg"]);
				rp.SPOT = Rating.ConvertDbVal2Double(battle["spotted"]);
				rp.FRAGS = Rating.ConvertDbVal2Double(battle["frags"]);
				rp.DEF = Rating.ConvertDbVal2Double(battle["def"]);
				rp.CAP = Rating.ConvertDbVal2Double(battle["cap"]);
				rp.WINS = Rating.ConvertDbVal2Double(battle["victory"]);
				rp.BATTLES = Rating.ConvertDbVal2Double(battle["battlesCount"]);
				// Calculate WN7
				string wn7 = Convert.ToInt32(Math.Round(Rating.CalculateWN7(rp, Rating.GetAverageBattleTier(), true), 0)).ToString();
				// Generate SQL to update WN7
				sql = "UPDATE battle SET wn7=" + wn7 + " WHERE id = " + battleId;
				DB.ExecuteNonQuery(sql);
			}
			dtBattles.Dispose();
			dtBattles.Clear();
		}

		public static double GetAverageBattleTier(string battleMode = "")
		{
			double tier = 0;
			// Get average battle tier, used for total player WN7 and battle WN7
			string battleModeWhere = "";
			if (battleMode != "")
			{
				battleModeWhere += " and ptb.battleMode=@battleMode ";
				DB.AddWithValue(ref battleModeWhere, "@battleMode", battleMode, DB.SqlDataType.VarChar);
			}
			string sql =
				"select sum(ptb.battles) as battles, sum(t.tier * ptb.battles) as tier " +
				"from playerTankBattle ptb left join " +
				"  playerTank pt on ptb.playerTankId=pt.id left join " +
				"  tank t on pt.tankId = t.id " +
				"where pt.playerId=@playerId " + battleModeWhere;
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DataTable dt = DB.FetchData(sql);
			if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
			{
				double totalBattles = Convert.ToDouble(dt.Rows[0][0]);
				if (totalBattles > 0)
				{
					tier = Convert.ToDouble(dt.Rows[0][1]) / totalBattles;
				}
			}
			return tier;
		}

		#endregion
	}
}
