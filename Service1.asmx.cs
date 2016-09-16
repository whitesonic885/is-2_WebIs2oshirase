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
	// �C������
	//--------------------------------------------------------------------------
	// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� 
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2oshirase")]

	public class Service1 : is2common.CommService
	{
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//		private static string sCRLF = "\\r\\n";
//		private static string sSepa = "|";
//		private static string sKanma = ",";
//		private static string sDbl = "\"";
//		private static string sSng = "'";
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END

		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * ���b�Z�[�W�̎擾
		 * �����F���[�U�[���
		 * �ߒl�F�X�e�[�^�X�A���b�Z�[�W
		 * 
		 *********************************************************************/
		[WebMethod]
		public string [] Get_Message(string [] sUser, string [] sKey) 
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�ꗗ�s�����m�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string [] sRet = new string [1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �f�[�^�擾
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			string sMessage = string.Empty;
			try
			{
				// �r�p�k��
				cmdQuery
					= "SELECT * \n"
					+  " FROM ( \n"
					+ "SELECT TRIM(CM18.�\��) \n"
					+  " FROM �b�l�P�W���m�点 CM18 \n"
					+ " WHERE CM18.�폜�e�f = '0' \n"
				    + " ORDER BY " 
					+  " CM18.�o�^�� DESC "
					+  ",CM18.�D�揇 DESC "
					+  ",CM18.\"�V�[�P���X�m�n\" DESC \n"
					+ ") \n"
					+ " WHERE rownum = 1 \n";

				// �f�[�^�ǂݎ��
				reader = CmdSelect(sUser, conn2, cmdQuery);
				sMessage = (reader.Read()) ? reader.GetString(0) : string.Empty;

				// �������ʃX�e�[�^�X�ݒ�
				sRet = new string [2];
				sRet[0] = "����I��";
				sRet[1] = sMessage;

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader �㏈��
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���m�点�ꗗ�s�����m�擾
		 * �����F���[�U�[���A�J�n�o�^���A�I���o�^���A�\��A�ڍד��e�A��ʉ���
		 *       [5]�@�\���A[6]�X���b�c�A[7]�\���e�f
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�o�^���A�\��A�V�[�P���X�m�n�j...
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Get_OshiraseTopN(string [] sUser, string [] sKey)
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�ꗗ�s�����m�擾�J�n");

// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
			string s����b�c   = sUser[0];
			string s���p�҂b�c = sUser[1];
			string s�@�\��     = (sKey.Length > 5) ? sKey[5] : "";
			string s�X���b�c   = (sKey.Length > 6) ? sKey[6] : "";
			string s�\���e�f   = (sKey.Length > 7) ? sKey[7] : "";
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string [] sRet = new string [1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �f�[�^�擾
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// �r�p�k��
				cmdQuery
					= "SELECT * \n"
					+  " FROM ( \n"
					+ "SELECT '|' "
					+     "|| SUBSTR(CM18.�o�^��, 1, 4) || '/' " 
					+     "|| SUBSTR(CM18.�o�^��, 5, 2) || '/' " 
					+     "|| SUBSTR(CM18.�o�^��, 7, 2) || '|' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					;
					if (sKey.Length >= 6){
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//						if (sKey[5].Equals("���m�点����")){
						if (s�@�\��.Equals("���m�点����")
							|| s�@�\��.Equals("���m�点�����Q")){
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
							cmdQuery += "|| DECODE(CM18.�Ǘ��ҋ敪,'2','�c�Ə�','���') || '|' ";
						}
					}
				cmdQuery = cmdQuery
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
					+     "|| TRIM(CM18.�\��) || '|' "
					+     "|| TRIM(CM18.�o�^��) || '|' "
					+     "|| TRIM(CM18.\"�V�[�P���X�m�n\") || '|' \n"
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					+     "|| TRIM(CM18.�Ǘ��ҋ敪) || '|' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
					;
				if(s�@�\��.Equals("���m�点�����Q")){ // �c�Ə�
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.����b�c) || '|' \n"
						+	"|| TRIM(NVL((SELECT CM01.����� FROM �b�l�O�P��� CM01 WHERE CM01.����b�c = CM18.����b�c),' ')) || '|' \n"
						+	"|| TRIM(CM18.�\��) || '|' \n"
						+	"|| DECODE(CM18.�\���e�f,'1','����','0','����',' ') || '|' \n"
						;
				}else if(s�@�\��.Equals("���m�点�{�^���׎�")){ // �׎�
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.�X���b�c) || '|' \n"
						+	"|| TRIM(CM18.����b�c) || '|' \n"
						;
				}else if(s�@�\��.Equals("���j���[�׎�")){ // �׎�
					cmdQuery = cmdQuery
						+	"|| TRIM(CM18.�X���b�c) || '|' \n"
						+	"|| TRIM(CM18.����b�c) || '|' \n"
						+	"|| TRIM(CM18.�ڍד��e) || '|' \n"
						+	"|| TRIM(CM18.\"���b�Z�[�W\") || '|' \n"
						+	"|| TRIM(CM18.�\�����ԊJ�n) || '|' \n"
						+	"|| TRIM(CM18.�\�����ԏI��) || '|' \n"
						+	"|| TRIM(CM18.�\���e�f) || '|' \n"
						+	"|| TO_CHAR(SYSDATE,'YYYYMMDD') || '|' \n"
						;
				}
				cmdQuery = cmdQuery
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
					+  " FROM �b�l�P�W���m�点 CM18 \n"
					+ " WHERE CM18.�폜�e�f = '0' \n";
				if ((sKey[0].Length > 0) && (sKey[1].Length > 0))
				{
					cmdQuery += " AND CM18.�o�^�� BETWEEN '" + sKey[0] + "' AND '" + sKey[1] + "' \n";
				}
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
//				if (sKey.Length < 6){
//					cmdQuery += " AND CM18.�Ǘ��ҋ敪 = '0' \n"; // �׎�
//				}else{
//					;
//				}
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
				if(sKey.Length < 6                       // �׎� ���m�点�\��(Ver 2.17�ȑO)
				 || s�@�\��.Equals("���m�点�{�^���׎�") // �׎� ���m�点�\��(Ver 2.18�ȍ~)
				 || s�@�\��.Equals("���j���[�׎�")       //     �V
				){
					cmdQuery += " AND CM18.�Ǘ��ҋ敪 = '0' \n"; // �׎�
					
					if(s�X���b�c.Length == 0){
						// ���p�҂̏����X���b�c�̎擾 �׎�(Ver 2.17�ȑO)
						string cmdQuery2
							= "SELECT CM14.�X���b�c \n"
							+ " FROM �b�l�P�S�X�֔ԍ� CM14 \n"
							+ ", �b�l�O�Q���� CM02, �b�l�O�S���p�� CM04 \n"
							+ " WHERE CM04.����b�c = '"+s����b�c+"' \n"
							+ " AND CM04.���p�҂b�c = '"+s���p�҂b�c+"' \n"
							+ " AND CM04.����b�c = CM02.����b�c \n"
							+ " AND CM04.����b�c = CM02.����b�c \n"
							+ " AND CM02.�X�֔ԍ� = CM14.�X�֔ԍ� \n"
							;
						reader = CmdSelect(sUser, conn2, cmdQuery2);
						if(reader.Read()){
							s�X���b�c = reader.GetString(0).TrimEnd();
						}
					}
					if(s�X���b�c.Length > 0){
						cmdQuery += " AND ( \n";
						cmdQuery += "   ( CM18.�X���b�c = ' ' \n";
						cmdQuery += "   AND CM18.����b�c = ' ' ) \n";
						cmdQuery += " OR \n";
						cmdQuery += "   ( CM18.�X���b�c = '"+s�X���b�c+"' \n";
						cmdQuery += "   AND CM18.����b�c IN (' ','"+s����b�c+"')) \n";
						cmdQuery += " ) \n";
					}else{
						cmdQuery += " AND CM18.�X���b�c = ' ' \n";
						cmdQuery += " AND CM18.����b�c = ' ' \n";
					}
				}else{
					if(s�@�\��.Equals("���m�点�����Q")){	 // �c�Ə� ���m�点�o�^
						cmdQuery += " AND CM18.�X���b�c = '"+s�X���b�c+"' \n";
					}else{									 // �Ǘ��� ���m�点�o�^
															 // �c�Ə� ���m�点�\��
						cmdQuery += " AND CM18.�X���b�c = ' ' \n";
						cmdQuery += " AND CM18.����b�c = ' ' \n";
					}
				}
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
				if (sKey[2].Length > 0)
				{
					cmdQuery += " AND CM18.�\�� LIKE '%" + sKey[2] + "%' \n";
				}
				if (sKey[3].Length > 0)
				{
					cmdQuery += " AND CM18.�ڍד��e LIKE '%" + sKey[3] + "%' \n";
				}
				cmdQuery += " ORDER BY " 
					+  " CM18.�o�^�� DESC "
					+  ",CM18.�D�揇 DESC "
					+  ",CM18.\"�V�[�P���X�m�n\" DESC \n"
					+ ") \n";
				if (sKey[4].Length > 0) 
				{
					cmdQuery += " WHERE rownum <= " + sKey[4] + " \n";
				}

				// �f�[�^�ǂݎ��
				reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				// �������ʃX�e�[�^�X�ݒ�
				sRet = new string [sList.Count + 1];
				if (sList.Count == 0) 
				{
					// �f�[�^��

					// �������ʃX�e�[�^�X�ݒ�
					sRet[0] = "�Y���f�[�^������܂���";
				}
				else
				{
					// �f�[�^�L

					// �f�[�^�擾
					int iCnt = 1;
					foreach (string row in sList) 
						sRet[iCnt++] = row;

					// �������ʃX�e�[�^�X�ݒ�
					sRet[0] = "����I��";
				}

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader �㏈��
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���m�点����
		 * �����F�o�^���A�V�[�P���X�m�n
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�\��A�ڍד��e�A�D�揇�A�V�[�P���X�m�n�A�X�V����
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Sel_Oshirase(string [] sUser, string [] sKey)
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�����J�n");

			OracleConnection conn2 = null;
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
//			string [] sRet = new string [9];
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//			string [] sRet = new string [10];
			string [] sRet = new string [17];
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �f�[�^����
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// �r�p�k��
				cmdQuery
					= "SELECT " 
					+       " CM18.�o�^�� "
					+       ",CM18.�\�� "
					+       ",CM18.�ڍד��e "
					+       ",CM18.�{�^���� "
					+       ",CM18.�A�h���X "
					+       ",CM18.�D�揇 "
					+       ",CM18.\"�V�[�P���X�m�n\" "
					+       ",CM18.�X�V���� \n"
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					+       ",CM18.�Ǘ��ҋ敪 \n"
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
					+       ",CM18.�X���b�c \n"
					+       ",CM18.����b�c \n"
					+       ",CM18.\"���b�Z�[�W\" \n"
					+       ",CM18.�\�����ԊJ�n \n"
					+       ",CM18.�\�����ԏI�� \n"
					+       ",CM18.�\���e�f \n"
					+       ",NVL((SELECT CM01.����� FROM �b�l�O�P��� CM01 WHERE CM01.����b�c = CM18.����b�c),' ') \n"
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
					+  " FROM �b�l�P�W���m�点 CM18 \n"
					+ " WHERE CM18.�o�^�� = '" + sKey[0] + "' \n" 
					+   " AND CM18.\"�V�[�P���X�m�n\" = '" + sKey[1] + "' \n"
					+   " AND CM18.�폜�e�f = '0' \n";

				// �f�[�^�ǂݎ��
				reader = CmdSelect(sUser, conn2, cmdQuery);
				if (reader.Read())
				{
					// �f�[�^�L

					// �f�[�^�擾
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
					sRet[5] = reader.GetString(4).Trim();
					sRet[6] = reader.GetString(5).Trim();
					sRet[7] = reader.GetString(6).Trim();
					sRet[8] = reader.GetDecimal(7).ToString().Trim();
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					sRet[9] = reader.GetString(8).Trim();
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
					sRet[10] = reader.GetString( 9).TrimEnd(); // �X���b�c
					sRet[11] = reader.GetString(10).TrimEnd(); // ����b�c
					sRet[12] = reader.GetString(11).TrimEnd(); // ���b�Z�[�W
					sRet[13] = reader.GetString(12).TrimEnd(); // �\�����ԊJ�n
					sRet[14] = reader.GetString(13).TrimEnd(); // �\�����ԏI��
					sRet[15] = reader.GetString(14).TrimEnd(); // �\���e�f
					sRet[16] = reader.GetString(15).TrimEnd(); // �����
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END

					// �������ʃX�e�[�^�X�ݒ�
					sRet[0] = "����I��";
				}
				else 
				{
					// �f�[�^��

					// �������ʃX�e�[�^�X�ݒ�
					sRet[0] = "�Y���f�[�^������܂���";
				}

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader �㏈��
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���m�点�ǉ�
		 * �����F�o�^���A�\��A�ڍד��e�A�{�^�����A�A�h���X�A�D�揇�A�X�V��
		 * �ߒl�F�X�e�[�^�X�A�X�V����
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Ins_Oshirase(string [] sUser, string [] sKey)
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�ǉ��J�n");
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
			string s�Ǘ��ҋ敪   = (sKey.Length >  7) ? sKey[ 7] : "0";
			string s�X���b�c     = (sKey.Length >  8) ? sKey[ 8] : " ";
			string s����b�c     = (sKey.Length >  9) ? sKey[ 9] : " ";
			string s���b�Z�[�W   = (sKey.Length > 10) ? sKey[10] : " ";
			string s�\�����ԊJ�n = (sKey.Length > 11) ? sKey[11] : " ";
			string s�\�����ԏI�� = (sKey.Length > 12) ? sKey[12] : " ";
			string s�\���e�f     = (s����b�c.Trim() == "") ? " " : "1";
			
			if(s���b�Z�[�W.Length == 0) s���b�Z�[�W = " ";
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END

			OracleConnection conn2 = null;
			string [] sRet = new string [2];
			string s�X�V���� = System.DateTime.Now.ToString("yyyyMMddHHmmss");

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �g�����U�N�V�����J�n
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// �f�[�^�ǉ�
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				// �r�p�k���i�V�K�ǉ�����o�^���̍s�����b�N�j
				cmdQuery
					= "SELECT �o�^�� "
					+   "FROM �b�l�P�W���m�点 "
					+  "WHERE �o�^�� = '" + sKey[0] + "' "
					+    "FOR UPDATE \n";

				// ���b�N
				reader = CmdSelect(sUser, conn2, cmdQuery);
				int iSeq = 0;
				while (reader.Read()) 
					iSeq++;
				// ����̃V�[�P���X�m�n����
				iSeq++;

				// �����m�F
				if (iSeq > 999) 
				{
					// �����I�[�o�[
					string errMsg = string.Format("�o�^�� {0} �̂��m�点���������āA�o�^�ł��܂���ł����B", sKey[0]);
					throw new Exception(errMsg);
				}

				// �r�p�k���i�V�K�ǉ��j
				cmdQuery
					= "INSERT INTO �b�l�P�W���m�点 \n"
//�ۗ� MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//					+ "( \n"
//					+ " �o�^��, \"�V�[�P���X�m�n\", �D�揇, �Ǘ��ҋ敪 \n"
//					+ ", �\��, �ڍד��e, �{�^����, �A�h���X \n"
//					+ ", �폜�e�f \n"
//					+ ", �o�^����, �o�^�o�f, �o�^�� \n"
//					+ ", �X�V����, �X�V�o�f, �X�V�� \n"
//					+ ") \n"
//�ۗ� MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
					+ " VALUES ( " 
					+         " '" + sKey[0] + "' " 
					+         ",'" + iSeq.ToString("000") + "' "
					+         ",'" + sKey[5].PadLeft(3, '0') + "' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
//					;
//				if(sKey.Length > 7){
//					cmdQuery += ",'" + sKey[7] + "' ";
//				}else{
//					cmdQuery += ",'0' ";
//				}
//				cmdQuery = cmdQuery
					+         ",'" + s�Ǘ��ҋ敪 + "' \n"
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
					+         ",'" + sKey[1] + "' "
					+         ",'" + sKey[2] + "' "
					+         ",'" + sKey[3] + "' "
					+         ",'" + sKey[4] + "' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					+         ",'" + s�X���b�c + "' \n"
					+         ",'" + s����b�c + "' \n"
					+         ",'" + s���b�Z�[�W + "' \n"
					+         ",'" + s�\�����ԊJ�n + "' \n"
					+         ",'" + s�\�����ԏI�� + "' \n"
//					+         ",'1' \n" // �\���e�f
					+         ",'" + s�\���e�f + "' \n"

// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
					+         ",'0' "
					+         ", " + s�X�V����
					+         ",'����o�^' "
					+         ",'" + sKey[6] + "' "
					+         ", " + s�X�V����
					+         ",'����o�^' "
					+         ",'" + sKey[6] + "' "
					+ " ) \n";

				// �f�[�^�ǉ�
				CmdUpdate(sUser, conn2, cmdQuery);

				// �R�~�b�g
				tran.Commit();

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "����I��";
				sRet[1] = s�X�V����;

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader �㏈��
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���m�点�X�V
		 * �����F�o�^���A�V�[�P���X�m�n�A�X�V�����A�\��A�ڍד��e�A�{�^�����A�A�h���X�A
		 * �@�@�@�D�揇�A�X�V�ҁA���o�^��
		 * �ߒl�F�X�e�[�^�X�A�X�V����
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Upd_Oshirase(string [] sUser, string [] sKey)
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�X�V�J�n");
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
			string s�Ǘ��ҋ敪   = (sKey.Length > 10) ? sKey[10] : "0";
			string s�X���b�c     = (sKey.Length > 11) ? sKey[11] : " ";
			string s����b�c     = (sKey.Length > 12) ? sKey[12] : " ";
			string s���b�Z�[�W   = (sKey.Length > 13) ? sKey[13] : " ";
			string s�\�����ԊJ�n = (sKey.Length > 14) ? sKey[14] : " ";
			string s�\�����ԏI�� = (sKey.Length > 15) ? sKey[15] : " ";
			string s�\���e�f     = (s����b�c.Trim() == "") ? " " : "1";

			if(s���b�Z�[�W.Length == 0) s���b�Z�[�W = " ";
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END

			OracleConnection conn2 = null;
			string [] sRet = new string [2];
			string s�X�V���� = System.DateTime.Now.ToString("yyyyMMddHHmmss");

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �g�����U�N�V�����J�n
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// �f�[�^�X�V
			OracleDataReader reader = null;
			string cmdQuery = string.Empty;
			try
			{
				if (sKey[0] == sKey[9]) 
				{
					// �o�^�����ύX����Ă��Ȃ��ꍇ

					// �r�p�k���i�X�V�j
					cmdQuery
						= "UPDATE �b�l�P�W���m�点 \n"
						+   " SET " 
						+       " �\�� = '"		+ sKey[3] + "' " 
						+       ",�ڍד��e = '"	+ sKey[4] + "' "
						+       ",�{�^���� = '"	+ sKey[5] + "' " 
						+       ",�A�h���X = '"	+ sKey[6] + "' "
						+       ",�D�揇 = '"	+ sKey[7].PadLeft(3, '0') + "' "
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
						+       ",�Ǘ��ҋ敪 = '"+ s�Ǘ��ҋ敪 + "' \n"
						+       ",�X���b�c   = '"+ s�X���b�c + "' \n"
						+       ",����b�c   = '"+ s����b�c + "' \n"
						+       ",\"���b�Z�[�W\" = '"+ s���b�Z�[�W + "' \n"
						+       ",�\�����ԊJ�n = '"+ s�\�����ԊJ�n + "' \n"
						+       ",�\�����ԏI�� = '"+ s�\�����ԏI�� + "' \n"
//						+       ",�\���e�f = '1' \n"
						+       ",�\���e�f = '"+ s�\���e�f + "' \n"
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
						+       ",�X�V���� = "	+ s�X�V����
						+       ",�X�V�o�f =	'����X�V' "
						+       ",�X�V�� = '"	+ sKey[8] + "' \n"
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
//						;
//					if(sKey.Length > 10){
//						cmdQuery += ",�Ǘ��ҋ敪 = '" + sKey[10] + "' ";
//					}
//					cmdQuery = cmdQuery
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
						+ " WHERE �o�^�� = '"	+ sKey[0] + "' \n" 
						+   " AND \"�V�[�P���X�m�n\" = '"  + sKey[1] + "' \n"
						+   " AND �X�V���� = "	+ sKey[2] + " \n"
						+   " AND �폜�e�f = '0' \n";

					// �f�[�^�X�V
					if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
					{
						// �X�V���s
						string errMsg = "���̒[���ōX�V����Ă��܂�";
						throw new Exception(errMsg);
					}
				}
				else 
				{
					// �o�^�����ύX����Ă���ꍇ

					// �r�p�k���i�폜�t���O�X�V�j
					cmdQuery
						= "UPDATE �b�l�P�W���m�点 \n"
						+    "SET " 
						+       " �폜�e�f = '1' "
						+       ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
						+       ",�X�V�o�f = '����X�V' "
						+       ",�X�V�� = '" + sKey[8] + "' \n"
						+ " WHERE �o�^�� = '" + sKey[9] + "' \n" 
						+   " AND \"�V�[�P���X�m�n\" = '" + sKey[1] + "' \n"
						+   " AND �X�V���� = " + sKey[2] + " \n";

					// �f�[�^�X�V
					if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
					{
						// �X�V���s
						string errMsg = "���̒[���ōX�V����Ă��܂�";
						throw new Exception(errMsg);
					}

					// �r�p�k���i�V�K�ǉ�����o�^���̍s�����b�N�j
					cmdQuery
						= "SELECT �o�^�� "
						+   "FROM �b�l�P�W���m�点 "
						+  "WHERE �o�^�� = '" + sKey[0] + "' "
						+    "FOR UPDATE \n";

					// �f�[�^�擾
					reader = CmdSelect(sUser, conn2, cmdQuery);
					int iSeq = 0;
					while (reader.Read()) 
						iSeq++;
					// ����̃V�[�P���X�m�n����
					iSeq++;

					// �����m�F
					if (iSeq > 999) 
					{
						// �����I�[�o�[
						string errMsg = string.Format("�o�^�� {0} �̂��m�点���������āA�o�^�ł��܂���ł����B", sKey[0]);
						throw new Exception(errMsg);
					}

					// �r�p�k���i�V�K�ǉ��j
					cmdQuery
						= "INSERT INTO �b�l�P�W���m�点 \n"
//�ۗ� MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//					+ "( \n"
//					+ " �o�^��, \"�V�[�P���X�m�n\", �D�揇, �Ǘ��ҋ敪 \n"
//					+ ", �\��, �ڍד��e, �{�^����, �A�h���X \n"
//					+ ", �폜�e�f \n"
//					+ ", �o�^����, �o�^�o�f, �o�^�� \n"
//					+ ", �X�V����, �X�V�o�f, �X�V�� \n"
//					+ ") \n"
//�ۗ� MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
						+ " VALUES ( " 
						+         " '" + sKey[0] + "' " 
						+         ",'" + iSeq.ToString("000") + "' "
						+         ",'" + sKey[7].PadLeft(3, '0') + "' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
//						;
//					if(sKey.Length > 10){
//						cmdQuery += ",'" + sKey[10] + "' ";
//					}else{
//						cmdQuery += ",'0' ";
//					}
//					cmdQuery = cmdQuery
					+         ",'" + s�Ǘ��ҋ敪 + "' \n"
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
						+         ",'" + sKey[3] + "' "
						+         ",'" + sKey[4] + "' "
						+         ",'" + sKey[5] + "' "
						+         ",'" + sKey[6] + "' "
// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� START
					+         ",'" + s�X���b�c + "' \n"
					+         ",'" + s����b�c + "' \n"
					+         ",'" + s���b�Z�[�W + "' \n"
					+         ",'" + s�\�����ԊJ�n + "' \n"
					+         ",'" + s�\�����ԏI�� + "' \n"
//					+         ",'1' \n" // �\���e�f
					+         ",'" + s�\���e�f + "' \n"

// MOD 2009.06.08 ���s�j���� �c�Ə��p���m�点�\���@�\�̒ǉ� END
						+         ",'0' "
						+         ", " + s�X�V����
						+         ",'����o�^' "
						+         ",'" + sKey[8] + "' "
						+         ", " + s�X�V����
						+         ",'����o�^' "
						+         ",'" + sKey[8] + "' "
						+ " ) \n";

					// �f�[�^�ǉ�
					CmdUpdate(sUser, conn2, cmdQuery);
				}

				// �R�~�b�g
				tran.Commit();

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "����I��";
				sRet[1] = s�X�V����;

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// Reader �㏈��
				if (reader != null) 
				{
					disposeReader(reader);
					reader = null;
				}
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���m�点�폜
		 * �����F�o�^���A�V�[�P���X�m�n�A�X�V�����A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Del_Oshirase(string [] sUser, string [] sKey)
		{
			// ���O�L�^
			logWriter(sUser, INF, "���m�点�폜�J�n");

			OracleConnection conn2 = null;
			string [] sRet = new string [1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if (conn2 == null)
			{
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �g�����U�N�V�����J�n
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			// �f�[�^�폜
			string cmdQuery = string.Empty;
			try
			{
				// �r�p�k���i�폜�t���O�X�V�j
				cmdQuery
					= "UPDATE �b�l�P�W���m�点 \n"
					+    "SET " 
					+       " �폜�e�f = '1' "
					+       ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",�X�V�o�f = '����X�V' "
					+       ",�X�V�� = '" + sKey[3] + "' \n"
					+ " WHERE �o�^�� = '" + sKey[0] + "' \n" 
					+   " AND \"�V�[�P���X�m�n\" = '" + sKey[1] + "' \n"
					+   " AND �X�V���� = " + sKey[2] + " \n";

				// �f�[�^�X�V
				if (CmdUpdate(sUser, conn2, cmdQuery) == 0)
				{
					// �X�V���s
					string errMsg = "���̒[���ōX�V����Ă��܂�";
					throw new Exception(errMsg);
				}

				// �R�~�b�g
				tran.Commit();

				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "����I��";

				// ���O�L�^
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = chgDBErrMsg(sUser, ex);
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				// ���̑��̃G���[����

				// ���[���o�b�N
				tran.Rollback();
				// �������ʃX�e�[�^�X�ݒ�
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				// ���O�L�^
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� START
		/*********************************************************************
		 * �\���e�f�X�V
		 * �����F�o�^���A�V�[�P���X�m�n�A�X�V�����A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string [] Upd_HyoujiFG(string [] sUser, string [] sKey)
		{
			logWriter(sUser, INF, "�\���e�f�X�V�J�n");

			OracleConnection conn2 = null;
			string [] sRet = new string[]{""};

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				// �ڑ����s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			// �g�����U�N�V�����J�n
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = string.Empty;
			try{
				cmdQuery
					= "UPDATE �b�l�P�W���m�点 \n"
					+    "SET " 
					+       " �\���e�f = '0' "
					+       ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",�X�V�o�f = '����X�V' "
					+       ",�X�V�� = '" + sKey[2] + "' \n"
					+ " WHERE �o�^�� = '" + sKey[0] + "' \n" 
					+   " AND \"�V�[�P���X�m�n\" = '" + sKey[1] + "' \n"
					;

				if(CmdUpdate(sUser, conn2, cmdQuery) == 0){
					// �X�V���s
//					string errMsg = "���̒[���ōX�V����Ă��܂�";
//					throw new Exception(errMsg);
				}
				tran.Commit();

				sRet[0] = "����I��";

				logWriter(sUser, INF, sRet[0]);
			}catch (OracleException ex){
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}catch (Exception ex){
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}finally{
				// �c�a�ؒf
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2010.06.29 ���s�j���� �c�Ə��p���m�点�o�^�@�\�̒ǉ� END
	}
}
