using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;

namespace is2oshirase
{
	/// <summary>
	/// [is2oshirase]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 
	//--------------------------------------------------------------------------
	// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2oshirase")]

	public class Service1 : is2common.CommService
	{
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//		private static string sCRLF = "\\r\\n";
//		private static string sSepa = "|";
//		private static string sKanma = ",";
//		private static string sDbl = "\"";
//		private static string sSng = "'";
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END

		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/*********************************************************************
		 * メッセージの取得
		 * 引数：ユーザー情報
		 * 戻値：ステータス、メッセージ
		 * 
		 *********************************************************************/
		[WebMethod]
		public string [] Get_Message(string [] sUser, string [] sKey) 
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ一覧ＴｏｐＮ取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string [] sRet = new string [1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// データ取得
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			string sMessage = string.Empty;
			try
			{
				// ＳＱＬ文
				cmdQuery
					= "SELECT * \n"
					+  " FROM ( \n"
					+ "SELECT TRIM(CM18.表題) \n"
					+  " FROM ＣＭ１８お知らせ CM18 \n"
					+ " WHERE CM18.削除ＦＧ = '0' \n"
				    + " ORDER BY " 
					+  " CM18.登録日 DESC "
					+  ",CM18.優先順 DESC "
					+  ",CM18.\"シーケンスＮＯ\" DESC \n"
					+ ") \n"
					+ " WHERE rownum = 1 \n";

				// データ読み取り
				reader = CmdSelect(sUser, conn2, cmdQuery);
				sMessage = (reader.Read()) ? reader.GetString(0) : string.Empty;

				// 処理結果ステータス設定
				sRet = new string [2];
				sRet[0] = "正常終了";
				sRet[1] = sMessage;

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader 後処理
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * お知らせ一覧ＴｏｐＮ取得
		 * 引数：ユーザー情報、開始登録日、終了登録日、表題、詳細内容、上位何個か
		 *       [5]機能名、[6]店所ＣＤ、[7]表示ＦＧ
		 * 戻値：ステータス、一覧（登録日、表題、シーケンスＮＯ）...
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Get_OshiraseTopN(string [] sUser, string [] sKey)
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ一覧ＴｏｐＮ取得開始");

// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
			string s会員ＣＤ   = sUser[0];
			string s利用者ＣＤ = sUser[1];
			string s機能名     = (sKey.Length > 5) ? sKey[5] : "";
			string s店所ＣＤ   = (sKey.Length > 6) ? sKey[6] : "";
			string s表示ＦＧ   = (sKey.Length > 7) ? sKey[7] : "";
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string [] sRet = new string [1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// データ取得
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// ＳＱＬ文
				cmdQuery
					= "SELECT * \n"
					+  " FROM ( \n"
					+ "SELECT '|' "
					+     "|| SUBSTR(CM18.登録日, 1, 4) || '/' " 
					+     "|| SUBSTR(CM18.登録日, 5, 2) || '/' " 
					+     "|| SUBSTR(CM18.登録日, 7, 2) || '|' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					;
					if (sKey.Length >= 6){
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//						if (sKey[5].Equals("お知らせ検索")){
						if (s機能名.Equals("お知らせ検索")
							|| s機能名.Equals("お知らせ検索２")){
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
							cmdQuery += "|| DECODE(CM18.管理者区分,'2','営業所','一般') || '|' ";
						}
					}
				cmdQuery = cmdQuery
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
					+     "|| TRIM(CM18.表題) || '|' "
					+     "|| TRIM(CM18.登録日) || '|' "
					+     "|| TRIM(CM18.\"シーケンスＮＯ\") || '|' \n"
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					+     "|| TRIM(CM18.管理者区分) || '|' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
					;
				if(s機能名.Equals("お知らせ検索２")){ // 営業所
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.会員ＣＤ) || '|' \n"
						+	"|| TRIM(NVL((SELECT CM01.会員名 FROM ＣＭ０１会員 CM01 WHERE CM01.会員ＣＤ = CM18.会員ＣＤ),' ')) || '|' \n"
						+	"|| TRIM(CM18.表題) || '|' \n"
						+	"|| DECODE(CM18.表示ＦＧ,'1','未読','0','既読',' ') || '|' \n"
						;
				}else if(s機能名.Equals("お知らせボタン荷主")){ // 荷主
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.店所ＣＤ) || '|' \n"
						+	"|| TRIM(CM18.会員ＣＤ) || '|' \n"
						;
				}else if(s機能名.Equals("メニュー荷主")){ // 荷主
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.店所ＣＤ) || '|' \n"
						+	"|| TRIM(CM18.会員ＣＤ) || '|' \n"
						+	"|| TRIM(CM18.詳細内容) || '|' \n"
						+	"|| TRIM(CM18.\"メッセージ\") || '|' \n"
						+	"|| TRIM(CM18.表示期間開始) || '|' \n"
						+	"|| TRIM(CM18.表示期間終了) || '|' \n"
						+	"|| TRIM(CM18.表示ＦＧ) || '|' \n"
						+	"|| TO_CHAR(SYSDATE,'YYYYMMDD') || '|' \n"
						;
				}
				cmdQuery = cmdQuery
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
					+  " FROM ＣＭ１８お知らせ CM18 \n"
					+ " WHERE CM18.削除ＦＧ = '0' \n";
				if ((sKey[0].Length > 0) && (sKey[1].Length > 0))
				{
					cmdQuery += " AND CM18.登録日 BETWEEN '" + sKey[0] + "' AND '" + sKey[1] + "' \n";
				}
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
//				if (sKey.Length < 6){
//					cmdQuery += " AND CM18.管理者区分 = '0' \n"; // 荷主
//				}else{
//					;
//				}
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
				if(sKey.Length < 6                       // 荷主 お知らせ表示(Ver 2.17以前)
				 || s機能名.Equals("お知らせボタン荷主") // 荷主 お知らせ表示(Ver 2.18以降)
				 || s機能名.Equals("メニュー荷主")       //     〃
				){
					cmdQuery += " AND CM18.管理者区分 = '0' \n"; // 荷主
					
					if(s店所ＣＤ.Length == 0){
						// 利用者の所属店所ＣＤの取得 荷主(Ver 2.17以前)
						string cmdQuery2
							= "SELECT CM14.店所ＣＤ \n"
							+ " FROM ＣＭ１４郵便番号 CM14 \n"
							+ ", ＣＭ０２部門 CM02, ＣＭ０４利用者 CM04 \n"
							+ " WHERE CM04.会員ＣＤ = '"+s会員ＣＤ+"' \n"
							+ " AND CM04.利用者ＣＤ = '"+s利用者ＣＤ+"' \n"
							+ " AND CM04.会員ＣＤ = CM02.会員ＣＤ \n"
							+ " AND CM04.部門ＣＤ = CM02.部門ＣＤ \n"
							+ " AND CM02.郵便番号 = CM14.郵便番号 \n"
							;
						reader = CmdSelect(sUser, conn2, cmdQuery2);
						if(reader.Read()){
							s店所ＣＤ = reader.GetString(0).TrimEnd();
						}
					}
					if(s店所ＣＤ.Length > 0){
						cmdQuery += " AND ( \n";
						cmdQuery += "   ( CM18.店所ＣＤ = ' ' \n";
						cmdQuery += "   AND CM18.会員ＣＤ = ' ' ) \n";
						cmdQuery += " OR \n";
						cmdQuery += "   ( CM18.店所ＣＤ = '"+s店所ＣＤ+"' \n";
						cmdQuery += "   AND CM18.会員ＣＤ IN (' ','"+s会員ＣＤ+"')) \n";
						cmdQuery += " ) \n";
					}else{
						cmdQuery += " AND CM18.店所ＣＤ = ' ' \n";
						cmdQuery += " AND CM18.会員ＣＤ = ' ' \n";
					}
				}else{
					if(s機能名.Equals("お知らせ検索２")){	 // 営業所 お知らせ登録
						cmdQuery += " AND CM18.店所ＣＤ = '"+s店所ＣＤ+"' \n";
					}else{									 // 管理者 お知らせ登録
															 // 営業所 お知らせ表示
						cmdQuery += " AND CM18.店所ＣＤ = ' ' \n";
						cmdQuery += " AND CM18.会員ＣＤ = ' ' \n";
					}
				}
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
				if (sKey[2].Length > 0)
				{
					cmdQuery += " AND CM18.表題 LIKE '%" + sKey[2] + "%' \n";
				}
				if (sKey[3].Length > 0)
				{
					cmdQuery += " AND CM18.詳細内容 LIKE '%" + sKey[3] + "%' \n";
				}
				cmdQuery += " ORDER BY " 
					+  " CM18.登録日 DESC "
					+  ",CM18.優先順 DESC "
					+  ",CM18.\"シーケンスＮＯ\" DESC \n"
					+ ") \n";
				if (sKey[4].Length > 0) 
				{
					cmdQuery += " WHERE rownum <= " + sKey[4] + " \n";
				}

				// データ読み取り
				reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				// 処理結果ステータス設定
				sRet = new string [sList.Count + 1];
				if (sList.Count == 0) 
				{
					// データ無

					// 処理結果ステータス設定
					sRet[0] = "該当データがありません";
				}
				else
				{
					// データ有

					// データ取得
					int iCnt = 1;
					foreach (string row in sList) 
						sRet[iCnt++] = row;

					// 処理結果ステータス設定
					sRet[0] = "正常終了";
				}

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader 後処理
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * お知らせ検索
		 * 引数：登録日、シーケンスＮＯ
		 * 戻値：ステータス、登録日、表題、詳細内容、優先順、シーケンスＮＯ、更新日時
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Sel_Oshirase(string [] sUser, string [] sKey)
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ検索開始");

			OracleConnection conn2 = null;
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
//			string [] sRet = new string [9];
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//			string [] sRet = new string [10];
			string [] sRet = new string [17];
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// データ検索
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// ＳＱＬ文
				cmdQuery
					= "SELECT " 
					+       " CM18.登録日 "
					+       ",CM18.表題 "
					+       ",CM18.詳細内容 "
					+       ",CM18.ボタン名 "
					+       ",CM18.アドレス "
					+       ",CM18.優先順 "
					+       ",CM18.\"シーケンスＮＯ\" "
					+       ",CM18.更新日時 \n"
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					+       ",CM18.管理者区分 \n"
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
					+       ",CM18.店所ＣＤ \n"
					+       ",CM18.会員ＣＤ \n"
					+       ",CM18.\"メッセージ\" \n"
					+       ",CM18.表示期間開始 \n"
					+       ",CM18.表示期間終了 \n"
					+       ",CM18.表示ＦＧ \n"
					+       ",NVL((SELECT CM01.会員名 FROM ＣＭ０１会員 CM01 WHERE CM01.会員ＣＤ = CM18.会員ＣＤ),' ') \n"
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
					+  " FROM ＣＭ１８お知らせ CM18 \n"
					+ " WHERE CM18.登録日 = '" + sKey[0] + "' \n" 
					+   " AND CM18.\"シーケンスＮＯ\" = '" + sKey[1] + "' \n"
					+   " AND CM18.削除ＦＧ = '0' \n";

				// データ読み取り
				reader = CmdSelect(sUser, conn2, cmdQuery);
				if (reader.Read())
				{
					// データ有

					// データ取得
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
					sRet[5] = reader.GetString(4).Trim();
					sRet[6] = reader.GetString(5).Trim();
					sRet[7] = reader.GetString(6).Trim();
					sRet[8] = reader.GetDecimal(7).ToString().Trim();
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					sRet[9] = reader.GetString(8).Trim();
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
					sRet[10] = reader.GetString( 9).TrimEnd(); // 店所ＣＤ
					sRet[11] = reader.GetString(10).TrimEnd(); // 会員ＣＤ
					sRet[12] = reader.GetString(11).TrimEnd(); // メッセージ
					sRet[13] = reader.GetString(12).TrimEnd(); // 表示期間開始
					sRet[14] = reader.GetString(13).TrimEnd(); // 表示期間終了
					sRet[15] = reader.GetString(14).TrimEnd(); // 表示ＦＧ
					sRet[16] = reader.GetString(15).TrimEnd(); // 会員名
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END

					// 処理結果ステータス設定
					sRet[0] = "正常終了";
				}
				else 
				{
					// データ無

					// 処理結果ステータス設定
					sRet[0] = "該当データがありません";
				}

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader 後処理
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * お知らせ追加
		 * 引数：登録日、表題、詳細内容、ボタン名、アドレス、優先順、更新者
		 * 戻値：ステータス、更新日時
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Ins_Oshirase(string [] sUser, string [] sKey)
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ追加開始");
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
			string s管理者区分   = (sKey.Length >  7) ? sKey[ 7] : "0";
			string s店所ＣＤ     = (sKey.Length >  8) ? sKey[ 8] : " ";
			string s会員ＣＤ     = (sKey.Length >  9) ? sKey[ 9] : " ";
			string sメッセージ   = (sKey.Length > 10) ? sKey[10] : " ";
			string s表示期間開始 = (sKey.Length > 11) ? sKey[11] : " ";
			string s表示期間終了 = (sKey.Length > 12) ? sKey[12] : " ";
			string s表示ＦＧ     = (s会員ＣＤ.Trim() == "") ? " " : "1";
			
			if(sメッセージ.Length == 0) sメッセージ = " ";
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END

			OracleConnection conn2 = null;
			string [] sRet = new string [2];
			string s更新日時 = System.DateTime.Now.ToString("yyyyMMddHHmmss");

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// トランザクション開始
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// データ追加
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// ＳＱＬ文（新規追加する登録日の行をロック）
				cmdQuery
					= "SELECT 登録日 "
					+   "FROM ＣＭ１８お知らせ "
					+  "WHERE 登録日 = '" + sKey[0] + "' "
					+    "FOR UPDATE \n";

				// ロック
				reader = CmdSelect(sUser, conn2, cmdQuery);
				int iSeq = 0;
				while (reader.Read()) 
					iSeq++;
				// 今回のシーケンスＮＯ生成
				iSeq++;

				// 件数確認
				if (iSeq > 999) 
				{
					// 件数オーバー
					string errMsg = string.Format("登録日 {0} のお知らせが多すぎて、登録できませんでした。", sKey[0]);
					throw new Exception(errMsg);
				}

				// ＳＱＬ文（新規追加）
				cmdQuery
					= "INSERT INTO ＣＭ１８お知らせ \n"
//保留 MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//					+ "( \n"
//					+ " 登録日, \"シーケンスＮＯ\", 優先順, 管理者区分 \n"
//					+ ", 表題, 詳細内容, ボタン名, アドレス \n"
//					+ ", 削除ＦＧ \n"
//					+ ", 登録日時, 登録ＰＧ, 登録者 \n"
//					+ ", 更新日時, 更新ＰＧ, 更新者 \n"
//					+ ") \n"
//保留 MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
					+ " VALUES ( " 
					+         " '" + sKey[0] + "' " 
					+         ",'" + iSeq.ToString("000") + "' "
					+         ",'" + sKey[5].PadLeft(3, '0') + "' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
//					;
//				if(sKey.Length > 7){
//					cmdQuery += ",'" + sKey[7] + "' ";
//				}else{
//					cmdQuery += ",'0' ";
//				}
//				cmdQuery = cmdQuery
					+         ",'" + s管理者区分 + "' \n"
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
					+         ",'" + sKey[1] + "' "
					+         ",'" + sKey[2] + "' "
					+         ",'" + sKey[3] + "' "
					+         ",'" + sKey[4] + "' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					+         ",'" + s店所ＣＤ + "' \n"
					+         ",'" + s会員ＣＤ + "' \n"
					+         ",'" + sメッセージ + "' \n"
					+         ",'" + s表示期間開始 + "' \n"
					+         ",'" + s表示期間終了 + "' \n"
//					+         ",'1' \n" // 表示ＦＧ
					+         ",'" + s表示ＦＧ + "' \n"

// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
					+         ",'0' "
					+         ", " + s更新日時
					+         ",'会員登録' "
					+         ",'" + sKey[6] + "' "
					+         ", " + s更新日時
					+         ",'会員登録' "
					+         ",'" + sKey[6] + "' "
					+ " ) \n";

				// データ追加
				CmdUpdate(sUser, conn2, cmdQuery);

				// コミット
				tran.Commit();

				// 処理結果ステータス設定
				sRet[0] = "正常終了";
				sRet[1] = s更新日時;

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader 後処理
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * お知らせ更新
		 * 引数：登録日、シーケンスＮＯ、更新日時、表題、詳細内容、ボタン名、アドレス、
		 * 　　　優先順、更新者、元登録日
		 * 戻値：ステータス、更新日時
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Upd_Oshirase(string [] sUser, string [] sKey)
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ更新開始");
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
			string s管理者区分   = (sKey.Length > 10) ? sKey[10] : "0";
			string s店所ＣＤ     = (sKey.Length > 11) ? sKey[11] : " ";
			string s会員ＣＤ     = (sKey.Length > 12) ? sKey[12] : " ";
			string sメッセージ   = (sKey.Length > 13) ? sKey[13] : " ";
			string s表示期間開始 = (sKey.Length > 14) ? sKey[14] : " ";
			string s表示期間終了 = (sKey.Length > 15) ? sKey[15] : " ";
			string s表示ＦＧ     = (s会員ＣＤ.Trim() == "") ? " " : "1";

			if(sメッセージ.Length == 0) sメッセージ = " ";
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END

			OracleConnection conn2 = null;
			string [] sRet = new string [2];
			string s更新日時 = System.DateTime.Now.ToString("yyyyMMddHHmmss");

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// トランザクション開始
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// データ更新
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				if (sKey[0] == sKey[9]) 
				{
					// 登録日が変更されていない場合

					// ＳＱＬ文（更新）
					cmdQuery
						= "UPDATE ＣＭ１８お知らせ \n"
						+   " SET " 
						+       " 表題 = '"		+ sKey[3] + "' " 
						+       ",詳細内容 = '"	+ sKey[4] + "' "
						+       ",ボタン名 = '"	+ sKey[5] + "' " 
						+       ",アドレス = '"	+ sKey[6] + "' "
						+       ",優先順 = '"	+ sKey[7].PadLeft(3, '0') + "' "
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
						+       ",管理者区分 = '"+ s管理者区分 + "' \n"
						+       ",店所ＣＤ   = '"+ s店所ＣＤ + "' \n"
						+       ",会員ＣＤ   = '"+ s会員ＣＤ + "' \n"
						+       ",\"メッセージ\" = '"+ sメッセージ + "' \n"
						+       ",表示期間開始 = '"+ s表示期間開始 + "' \n"
						+       ",表示期間終了 = '"+ s表示期間終了 + "' \n"
//						+       ",表示ＦＧ = '1' \n"
						+       ",表示ＦＧ = '"+ s表示ＦＧ + "' \n"
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
						+       ",更新日時 = "	+ s更新日時
						+       ",更新ＰＧ =	'会員更新' "
						+       ",更新者 = '"	+ sKey[8] + "' \n"
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
//						;
//					if(sKey.Length > 10){
//						cmdQuery += ",管理者区分 = '" + sKey[10] + "' ";
//					}
//					cmdQuery = cmdQuery
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
						+ " WHERE 登録日 = '"	+ sKey[0] + "' \n" 
						+   " AND \"シーケンスＮＯ\" = '"  + sKey[1] + "' \n"
						+   " AND 更新日時 = "	+ sKey[2] + " \n"
						+   " AND 削除ＦＧ = '0' \n";

					// データ更新
					if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
					{
						// 更新失敗
						string errMsg = "他の端末で更新されています";
						throw new Exception(errMsg);
					}
				}
				else 
				{
					// 登録日が変更されている場合

					// ＳＱＬ文（削除フラグ更新）
					cmdQuery
						= "UPDATE ＣＭ１８お知らせ \n"
						+    "SET " 
						+       " 削除ＦＧ = '1' "
						+       ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
						+       ",更新ＰＧ = '会員更新' "
						+       ",更新者 = '" + sKey[8] + "' \n"
						+ " WHERE 登録日 = '" + sKey[9] + "' \n" 
						+   " AND \"シーケンスＮＯ\" = '" + sKey[1] + "' \n"
						+   " AND 更新日時 = " + sKey[2] + " \n";

					// データ更新
					if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
					{
						// 更新失敗
						string errMsg = "他の端末で更新されています";
						throw new Exception(errMsg);
					}

					// ＳＱＬ文（新規追加する登録日の行をロック）
					cmdQuery
						= "SELECT 登録日 "
						+   "FROM ＣＭ１８お知らせ "
						+  "WHERE 登録日 = '" + sKey[0] + "' "
						+    "FOR UPDATE \n";

					// データ取得
					reader = CmdSelect(sUser, conn2, cmdQuery);
					int iSeq = 0;
					while (reader.Read()) 
						iSeq++;
					// 今回のシーケンスＮＯ生成
					iSeq++;

					// 件数確認
					if (iSeq > 999) 
					{
						// 件数オーバー
						string errMsg = string.Format("登録日 {0} のお知らせが多すぎて、登録できませんでした。", sKey[0]);
						throw new Exception(errMsg);
					}

					// ＳＱＬ文（新規追加）
					cmdQuery
						= "INSERT INTO ＣＭ１８お知らせ \n"
//保留 MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//					+ "( \n"
//					+ " 登録日, \"シーケンスＮＯ\", 優先順, 管理者区分 \n"
//					+ ", 表題, 詳細内容, ボタン名, アドレス \n"
//					+ ", 削除ＦＧ \n"
//					+ ", 登録日時, 登録ＰＧ, 登録者 \n"
//					+ ", 更新日時, 更新ＰＧ, 更新者 \n"
//					+ ") \n"
//保留 MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
						+ " VALUES ( " 
						+         " '" + sKey[0] + "' " 
						+         ",'" + iSeq.ToString("000") + "' "
						+         ",'" + sKey[7].PadLeft(3, '0') + "' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
//						;
//					if(sKey.Length > 10){
//						cmdQuery += ",'" + sKey[10] + "' ";
//					}else{
//						cmdQuery += ",'0' ";
//					}
//					cmdQuery = cmdQuery
					+         ",'" + s管理者区分 + "' \n"
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
						+         ",'" + sKey[3] + "' "
						+         ",'" + sKey[4] + "' "
						+         ",'" + sKey[5] + "' "
						+         ",'" + sKey[6] + "' "
// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 START
					+         ",'" + s店所ＣＤ + "' \n"
					+         ",'" + s会員ＣＤ + "' \n"
					+         ",'" + sメッセージ + "' \n"
					+         ",'" + s表示期間開始 + "' \n"
					+         ",'" + s表示期間終了 + "' \n"
//					+         ",'1' \n" // 表示ＦＧ
					+         ",'" + s表示ＦＧ + "' \n"

// MOD 2009.06.08 東都）高木 営業所用お知らせ表示機能の追加 END
						+         ",'0' "
						+         ", " + s更新日時
						+         ",'会員登録' "
						+         ",'" + sKey[8] + "' "
						+         ", " + s更新日時
						+         ",'会員登録' "
						+         ",'" + sKey[8] + "' "
						+ " ) \n";

					// データ追加
					CmdUpdate(sUser, conn2, cmdQuery);
				}

				// コミット
				tran.Commit();

				// 処理結果ステータス設定
				sRet[0] = "正常終了";
				sRet[1] = s更新日時;

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader 後処理
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * お知らせ削除
		 * 引数：登録日、シーケンスＮＯ、更新日時、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Del_Oshirase(string [] sUser, string [] sKey)
		{
			// ログ記録
			logWriter(sUser, INF, "お知らせ削除開始");

			OracleConnection conn2 = null;
			string [] sRet = new string [1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// トランザクション開始
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// データ削除
			string cmdQuery = string.Empty;
			try
			{
				// ＳＱＬ文（削除フラグ更新）
				cmdQuery
					= "UPDATE ＣＭ１８お知らせ \n"
					+    "SET " 
					+       " 削除ＦＧ = '1' "
					+       ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",更新ＰＧ = '会員更新' "
					+       ",更新者 = '" + sKey[3] + "' \n"
					+ " WHERE 登録日 = '" + sKey[0] + "' \n" 
					+   " AND \"シーケンスＮＯ\" = '" + sKey[1] + "' \n"
					+   " AND 更新日時 = " + sKey[2] + " \n";

				// データ更新
				if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
				{
					// 更新失敗
					string errMsg = "他の端末で更新されています";
					throw new Exception(errMsg);
				}

				// コミット
				tran.Commit();

				// 処理結果ステータス設定
				sRet[0] = "正常終了";

				// ログ記録
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// その他のエラー発生

				// ロールバック
				tran.Rollback();
				// 処理結果ステータス設定
				sRet[0] = "サーバエラー：" + ex.Message;
				// ログ記録
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 START
		/*********************************************************************
		 * 表示ＦＧ更新
		 * 引数：登録日、シーケンスＮＯ、更新日時、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Upd_HyoujiFG(string [] sUser, string [] sKey)
		{
			logWriter(sUser, INF, "表示ＦＧ更新開始");

			OracleConnection conn2 = null;
			string [] sRet = new string[]{""};

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				// 接続失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			// トランザクション開始
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = string.Empty;
			try{
				cmdQuery
					= "UPDATE ＣＭ１８お知らせ \n"
					+    "SET " 
					+       " 表示ＦＧ = '0' "
					+       ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",更新ＰＧ = '会員更新' "
					+       ",更新者 = '" + sKey[2] + "' \n"
					+ " WHERE 登録日 = '" + sKey[0] + "' \n" 
					+   " AND \"シーケンスＮＯ\" = '" + sKey[1] + "' \n"
					;

				if(CmdUpdate(sUser, conn2, cmdQuery) == 0){
					// 更新失敗
//					string errMsg = "他の端末で更新されています";
//					throw new Exception(errMsg);
				}
				tran.Commit();

				sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}catch (OracleException ex){
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}catch (Exception ex){
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}finally{
				// ＤＢ切断
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2010.06.29 東都）高木 営業所用お知らせ登録機能の追加 END
	}
}
