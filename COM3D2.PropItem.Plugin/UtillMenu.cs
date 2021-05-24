using BepInEx.Logging;
using MaidStatus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.PropItem.Plugin
{
    class UtillMenu
    {
        public static ManualLogSource logger;


        public UtillMenu()
        {

        }
		/*
        public static string[] menuList;

        public static void SetMenuList()
        {
            menuList = GameUty.FileSystemMod.GetList(string.Empty, AFileSystemBase.ListType.AllFolder);
            logger.LogMessage("SetMenuList 0 " + menuList[0]);
        }
        */
		public static bool isLoadMenuFiles = false;
        public static Dictionary<int, SMenuItem> m_menuRidDic;
        public static Dictionary<MPN, List<SMenuItem>> menuRidDic=new Dictionary<MPN, List<SMenuItem>>();
		public static List<SMenuItem> m_listMember;
		public static EditItemTextureCache editItemTextureCache;
		public static bool m_bGroupLeader;

		internal static void initList()
        {
            foreach (var item in (MPN[])Enum.GetValues(typeof(MPN)))
            {
				menuRidDic.Add(item, new List<SMenuItem>());
            }
		}
		
		internal static void fixList()
        {
            foreach (var item in (MPN[])Enum.GetValues(typeof(MPN)))
            {
				if (menuRidDic[item].Count == 0)
					menuRidDic.Remove(item);
			}
		}

		public static void InitMenuNative()
		{
			logger.LogMessage("=== InitMenu ===");
			if (isLoadMenuFiles)
			{
				return;
			}
			isLoadMenuFiles = true;
			Stopwatch stopwatch = new Stopwatch(); //객체 선언
			stopwatch.Start(); // 시간측정 시작
			logger.LogMessage("InitMenu st : " + string.Format("{0:0.000} ", stopwatch.Elapsed.ToString()));

			#region

			//editItemTextureCache = SceneEdit.Instance.editItemTextureCache;
			editItemTextureCache = PropItem.instance.gameObject.GetComponent<EditItemTextureCache>();
			if (!editItemTextureCache)
            {
				editItemTextureCache = PropItem.instance.gameObject.AddComponent<EditItemTextureCache>();
            }
			//while (GameMain.Instance.CharacterMgr.IsBusy())
			//{
			//	yield return null;
			//}
			MenuDataBase menuDataBase = GameMain.Instance.MenuDataBase;
			//while (!menuDataBase.JobFinished())
			//{
			//	yield return null;
			//}

			//TODO
			//InitCategoryList();

			int fileCount = menuDataBase.GetDataSize();
			logger.LogMessage(" fileCount : " + fileCount);

			List <SMenuItem> menuList = new List<SMenuItem>(fileCount);
			m_menuRidDic = new Dictionary<int, SMenuItem>(fileCount);
			Dictionary<int, List<int>> menuGroupMemberDic = new Dictionary<int, List<int>>();
			//float time = Time.realtimeSinceStartup;

			// arc 안에 있는 공식 파일
			for (int i = 0; i < fileCount; i++)
			{				
				menuDataBase.SetIndex(i);
				string fileName = menuDataBase.GetMenuFileName();
				string parent_filename = menuDataBase.GetParentMenuFileName();

				//logger.LogMessage("fileName1 : " + fileName);
				//logger.LogMessage("parent_filename : " + parent_filename);

				//continue;


				if (GameMain.Instance.CharacterMgr.status.IsHavePartsItem(fileName))
				{
					SMenuItem mi = new SMenuItem();
					mi.m_strMenuFileName = fileName;
					mi.m_nMenuFileRID = fileName.GetHashCode();
					try
					{
						ReadMenuItemDataFromNative(mi, i);
					}
					catch (Exception ex)
					{
						logger.LogError(string.Concat(new string[]
						{
						"ReadMenuItemDataFromNative 例外／",
						fileName,
						"／",
						ex.Message,
						" StackTrace／",
						ex.StackTrace
						}));
					}
					if (!mi.m_bMan && editItemTextureCache.IsRegister(mi.m_nMenuFileRID))
					{
						//AddMenuItemToList(mi);
						menuList.Add(mi);
						addList(mi);
						/*
						if (!m_menuRidDic.ContainsKey(mi.m_nMenuFileRID))
						{
							m_menuRidDic.Add(mi.m_nMenuFileRID, mi);
							
						}
						*/
						string parentMenuName = GetParentMenuFileName(mi);
						if (!string.IsNullOrEmpty(parentMenuName))
						{
							int hashCode = parentMenuName.GetHashCode();
							if (!menuGroupMemberDic.ContainsKey(hashCode))
							{
								menuGroupMemberDic.Add(hashCode, new List<int>());
							}
							menuGroupMemberDic[hashCode].Add(mi.m_strMenuFileName.ToLower().GetHashCode());
						}
						else if (mi.m_strCateName.IndexOf("set_") != -1 && mi.m_strMenuFileName.IndexOf("_del") == -1)
						{
							mi.m_bGroupLeader = true;
							mi.m_listMember = new List<SMenuItem>();
							mi.m_listMember.Add(mi);
						}
						//if (0.5f < Time.realtimeSinceStartup - time)
						//{
						//	yield return null;
						//	time = Time.realtimeSinceStartup;
						//}
					}
				}
			}

			// 모드 폴더 파일. 이시점에서 오류 발생중
			foreach (string strFileName in GameUty.ModOnlysMenuFiles)
			{
				logger.LogMessage("strFileName : " + strFileName);

				//continue;

				SMenuItem mi2 = new SMenuItem();
				if (GetMenuItemSetUP(mi2, strFileName, false))
				{
					if ((!mi2.m_bMan) && (mi2.m_texIcon!= null))
					{
						//AddMenuItemToList(mi2);
						menuList.Add(mi2);
						addList(mi2);
						/*
						if (!m_menuRidDic.ContainsKey(mi2.m_nMenuFileRID))
						{
							m_menuRidDic.Add(mi2.m_nMenuFileRID, mi2);
						}
						*/
						string parentMenuName2 = GetParentMenuFileName(mi2);
						if (!string.IsNullOrEmpty(parentMenuName2))
						{
							int hashCode2 = parentMenuName2.GetHashCode();
							if (!menuGroupMemberDic.ContainsKey(hashCode2))
							{
								menuGroupMemberDic.Add(hashCode2, new List<int>());
							}
							menuGroupMemberDic[hashCode2].Add(mi2.m_strMenuFileName.ToLower().GetHashCode());
						}
						else if (mi2.m_strCateName.IndexOf("set_") != -1 && mi2.m_strMenuFileName.IndexOf("_del") == -1)
						{
							m_bGroupLeader = true;
							m_listMember = new List<SMenuItem>();
							m_listMember.Add(mi2);
						}
						//if (0.5f < Time.realtimeSinceStartup - time)
						//{
						//	yield return null;
						//	time = Time.realtimeSinceStartup;
						//}
					}
				}
			}

			//TODO  FixedInitMenu
			//FixedInitMenu(menuList, m_menuRidDic, menuGroupMemberDic);


			//yield return base.StartCoroutine(this.FixedInitMenu(menuList, this.m_menuRidDic, menuGroupMemberDic));
			//yield return base.StartCoroutine(this.CoLoadWait());
			//yield break;

			#endregion

			fixList();

			logger.LogMessage("InitMenu ed : " + string.Format("{0:0.000} ", stopwatch.Elapsed.ToString()));
			isLoadMenuFiles = false;
		}

		internal static void addList(SMenuItem mi)
        {
			if (!m_menuRidDic.ContainsKey(mi.m_nMenuFileRID))
			{
				m_menuRidDic.Add(mi.m_nMenuFileRID, mi);
			}
			menuRidDic[mi.m_mpn].Add(mi);
			/*
			if (!menuRidDic.ContainsKey(mi.m_mpn))
			{
				menuRidDic.Add(mi.m_mpn, new List<SMenuItem>());
			}
			*/
		}

		/*
		internal static void InitMenu()
        {
			logger.LogMessage("=== InitMenu ===");
			if (isLoadMenuFiles)
            {
                return;
            }
            isLoadMenuFiles = true;
            Stopwatch stopwatch = new Stopwatch(); //객체 선언
            stopwatch.Start(); // 시간측정 시작
            logger.LogMessage("InitMenu st " + string.Format("{0:0.000} ", stopwatch.Elapsed.ToString()));

            #region

            this.InitCategoryList();
            int fileCount = GameMain.Instance.MenuDataBase.GetDataSize();
            logger.LogMessage("MenuDataBase : " + fileCount);

            string[] files = GameUty.MenuFiles;
            logger.LogMessage("GameUty.MenuFiles : " + files.Length);
            logger.LogMessage("GameUty.MenuFiles : " + files[0]);

            List<SMenuItem> menuList = new List<SMenuItem>(fileCount);
            m_menuRidDic = new Dictionary<int, SMenuItem>(fileCount);
            Dictionary<int, List<int>> menuGroupMemberDic = new Dictionary<int, List<int>>();

            foreach (string strFileName in files)
            {
				logger.LogDebug("InitMenu : " + strFileName);
                if (!strFileName.ToLower().Contains("/man"))
                {
                    if (GameMain.Instance.CharacterMgr.status.IsHavePartsItem(Path.GetFileName(strFileName)))
                    {
                        SMenuItem mi = new SMenuItem();
						if (GetMenuItemSetUP(mi, strFileName, false))
                        {
							if (!mi.m_bMan && !(mi.m_texIconRef == null))
                            {

                            }

						}
                    }
                }
            }
            #endregion

            logger.LogMessage("InitMenu ed " + string.Format("{0:0.000} ", stopwatch.Elapsed.ToString()));
            isLoadMenuFiles = false;
        }
		*/

		/// <summary>
		/// 모드 전용
		/// </summary>
		/// <param name="mi"></param>
		/// <param name="f_strMenuFileName"></param>
		/// <param name="f_bMan"></param>
		/// <returns></returns>
		public static bool GetMenuItemSetUP(SMenuItem mi, string f_strMenuFileName, bool f_bMan = false)
        {
            if (f_strMenuFileName.Contains("_zurashi"))
            {
                return false;
            }
            if (f_strMenuFileName.Contains("_mekure"))
            {
                return false;
            }
            f_strMenuFileName = Path.GetFileName(f_strMenuFileName);
            mi.m_strMenuFileName = f_strMenuFileName;
            mi.m_nMenuFileRID = f_strMenuFileName.ToLower().GetHashCode();
            try
            {
                if (!InitMenuItemScript(mi, f_strMenuFileName, f_bMan))
                {
					logger.LogWarning( "메뉴 스크립트를 읽을 수 없습니다。" + f_strMenuFileName);
                }
            }
            catch (Exception ex)
            {
				logger.LogWarning(string.Concat(new string[]
                {
                "GetMenuItemSetUP 例外／",
                f_strMenuFileName,
                "／",
                ex.Message,
                " StackTrace／",
                ex.StackTrace
                }));
                return false;
            }
            return true;
        }


		private static byte[] m_byItemFileBuffer;

		/// <summary>
		/// 모드 전용
		/// </summary>
		/// <param name="mi"></param>
		/// <param name="f_strMenuFileName"></param>
		/// <param name="f_bMan"></param>
		/// <returns></returns>
		public static bool InitMenuItemScript(SMenuItem mi, string f_strMenuFileName, bool f_bMan)
		{
			if (f_strMenuFileName.IndexOf("mod_") == 0)
			{
				string modPathFileName = Menu.GetModPathFileName(f_strMenuFileName);
				return !string.IsNullOrEmpty(modPathFileName) && InitModMenuItemScript(mi, modPathFileName);
			}
			try
			{
				using (AFileBase afileBase = GameUty.FileOpen(f_strMenuFileName, null))
				{
					NDebug.Assert(afileBase.IsValid(), "メニューファイルが存在しません。 :" + f_strMenuFileName);
					if (m_byItemFileBuffer == null)
					{
						m_byItemFileBuffer = new byte[System.Math.Max(500000, afileBase.GetSize())];
					}
					else if (m_byItemFileBuffer.Length < afileBase.GetSize())
					{
						m_byItemFileBuffer = new byte[afileBase.GetSize()];
					}
					afileBase.Read(ref m_byItemFileBuffer, afileBase.GetSize());
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Concat(new string[]
				{
				"メニューファイルがが読み込めませんでした。 : ",
				f_strMenuFileName,
				" : ",
				ex.Message,
				" : StackTrace ：\n",
				ex.StackTrace
				}));
				throw ex;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(m_byItemFileBuffer), Encoding.UTF8);
			string text = binaryReader.ReadString();
            if (text != "CM3D2_MENU")
            {
				logger.LogWarning("ProcScriptBin 例外 : 헤더 파일이 손상되었습니다。" + f_strMenuFileName);
				return false;
            }
			int num = binaryReader.ReadInt32();
			string path = binaryReader.ReadString();
			string text2 = binaryReader.ReadString();
			string text3 = binaryReader.ReadString();
			string text4 = binaryReader.ReadString();
			long num2 = (long)binaryReader.ReadInt32();
			int num3 = 0;
			string text5 = null;
			string text6 = string.Empty;
			string text7 = string.Empty;
			try
			{
				for (; ; )
				{
					int num4 = (int)binaryReader.ReadByte();
					text7 = text6;
					text6 = string.Empty;
					if (num4 == 0)
					{
						break;
					}
					for (int i = 0; i < num4; i++)
					{
						text6 = text6 + "\"" + binaryReader.ReadString() + "\" ";
					}
					if (!(text6 == string.Empty))
					{
						string stringCom = UTY.GetStringCom(text6);
						string[] stringList = UTY.GetStringList(text6);
						if (stringCom == "name")
						{
							string text8 = stringList[1];
							string text9 = string.Empty;
							string arg = string.Empty;
							int j = 0;
							while (j < text8.Length && text8[j] != '\u3000' && text8[j] != ' ')
							{
								text9 += text8[j];
								j++;
							}
							while (j < text8.Length)
							{
								arg += text8[j];
								j++;
							}
							mi.m_strMenuName = text9;
						}
						else if (stringCom == "setumei")
						{
							//mi.m_strInfo = stringList[1];
							//mi.m_strInfo = mi.m_strInfo.Replace("《改行》", "\n");
						}
						else if (stringCom == "category")
						{
							string strCateName = stringList[1].ToLower();
							mi.m_strCateName = strCateName;
							try
							{
								mi.m_mpn = (MPN)Enum.Parse(typeof(MPN), mi.m_strCateName);
							}
							catch
							{
								logger.LogWarning("카테고리가 없습니다1。" + mi.m_strCateName);
								mi.m_mpn = MPN.null_mpn;
							}
						}
						else if (stringCom == "color_set")
						{
							try
							{
								mi.m_eColorSetMPN = (MPN)Enum.Parse(typeof(MPN), stringList[1].ToLower());
							}
							catch
							{
								logger.LogWarning("카테고리가 없습니다2。" + mi.m_strCateName);
							}
							if (stringList.Length >= 3)
							{
								mi.m_strMenuNameInColorSet = stringList[2].ToLower();
							}
						}
						else if (stringCom == "tex" || stringCom == "テクスチャ変更")// 텍스처 변경
						{
							MaidParts.PARTS_COLOR pcMultiColorID = MaidParts.PARTS_COLOR.NONE;
							if (stringList.Length == 6)
							{
								string text10 = stringList[5];
								try
								{
									pcMultiColorID = (MaidParts.PARTS_COLOR)Enum.Parse(typeof(MaidParts.PARTS_COLOR), text10.ToUpper());
								}
								catch
								{
									logger.LogWarning("무한 색 ID가 없습니다。" + text10);
								}
								mi.m_pcMultiColorID = pcMultiColorID;
							}
						}
						else if (stringCom == "icon" || stringCom == "icons")
						{
							text5 = stringList[1];
						}
						
						else if (!(stringCom == "iconl"))
						{
							if (!(stringCom == "setstr"))
							{
								if (!(stringCom == "アイテムパラメータ"))
								{
									//if (stringCom == "saveitem")
									//{
									//	string text11 = stringList[1];
									//	if (text11 == string.Empty)
									//	{
									//		UnityEngine.Debug.LogError("err SaveItem \"" + text11);
									//	}
									//	if (text11 == null)
									//	{
									//		UnityEngine.Debug.LogError("err SaveItem null=\"" + text11);
									//	}
									//	if (text11 != string.Empty)
									//	{
									//	}
									//}else
									if (!(stringCom == "catno"))
									{
										//if (stringCom == "additem")
										//{
										//	num3++;
										//}
										//else if (stringCom == "unsetitem")
										//{
										//	mi.m_boDelOnly = true;
										//}else 
										if (stringCom == "priority")
										{
											mi.m_fPriority = float.Parse(stringList[1]);
										}else
										if (stringCom == "メニューフォルダ")
										{
											if (stringList[1].ToLower() == "man")
											{
												mi.m_bMan = true;
											}
										}
										//else if (stringCom == "collabo")
										//{
										//	mi.m_collabo = true;
										//}
									}
								}
							}
						}/*
						*/
					}
				}
			}
			catch (Exception ex2)
			{
				logger.LogError(string.Concat(new string[]
				{
				"Exception ",
				Path.GetFileName(path),
				" 현재 진행 중이던 행 = ",
				text6,
				" 이전 행 = ",
				text7,
				"   ",
				ex2.Message,
				"StackTrace：\n",
				ex2.StackTrace
				}));
				throw ex2;
			}
			if (!string.IsNullOrEmpty(text5))
			{
				editItemTextureCache.PreLoadRegister(mi.m_nMenuFileRID, text5);
				//logger.LogDebug("ImportCM.CreateTexture : " + mi.m_nMenuFileRID + " , " + mi.m_strMenuName + " , " + text5);
				//try
				//{
				//	mi.m_texIcon = ImportCM.CreateTexture(text5);
				//}
				//catch (Exception)
				//{
				//	logger.LogError("ImportCM.CreateTexture : " + f_strMenuFileName+" , "+ text5);
				//}
			}
			binaryReader.Close();
			return true;
		}

		public static bool InitModMenuItemScript(SMenuItem mi, string f_strModFileName)
		{
			byte[] buffer = null;
			try
			{
				using (FileStream fileStream = new FileStream(f_strModFileName, FileMode.Open))
				{
					if (fileStream == null)
					{
						logger.LogWarning("MODアイテムメニューファイルが見つかりません。");
						return false;
					}
					buffer = new byte[fileStream.Length];
					fileStream.Read(buffer, 0, (int)fileStream.Length);
				}
			}
			catch (Exception ex)
			{
				logger.LogError("InitModMenuItemScript MODアイテムメニューファイルが読み込めませんでした。 : " + f_strModFileName + " : " + ex.Message);
				return false;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer), Encoding.UTF8);
			string text = binaryReader.ReadString();
            if (text != "CM3D2_MOD")
            {
				logger.LogWarning( "InitModMenuItemScript 例外 : ヘッダーファイルが不正です。" + text);
				return false;
			}
			int num = binaryReader.ReadInt32();
			string text2 = binaryReader.ReadString();
			string text3 = binaryReader.ReadString();
			string strMenuName = binaryReader.ReadString();
			string strCateName = binaryReader.ReadString();
			string text4 = binaryReader.ReadString();
			string text5 = binaryReader.ReadString();
			MPN mpn = MPN.null_mpn;
			try
			{
				mpn = (MPN)Enum.Parse(typeof(MPN), text5);
			}
			catch
			{
				NDebug.Assert("カテゴリがありません。" + text5, false);
			}
			string text6 = string.Empty;
			if (mpn != MPN.null_mpn)
			{
				text6 = binaryReader.ReadString();
			}
			string s = binaryReader.ReadString();
			int num2 = binaryReader.ReadInt32();
			Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
			for (int i = 0; i < num2; i++)
			{
				string key = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				byte[] value = binaryReader.ReadBytes(count);
				dictionary.Add(key, value);
			}
			binaryReader.Close();
			binaryReader = null;
			mi.m_bMod = true;
			mi.m_strMenuFileName = Path.GetFileName(f_strModFileName);
			mi.m_nMenuFileRID = mi.m_strMenuFileName.ToLower().GetHashCode();
			mi.m_strMenuName = strMenuName;
			//mi.m_strInfo = text4.Replace("《改行》", "\n");
			mi.m_strCateName = strCateName;
			try
			{
				mi.m_mpn = (MPN)Enum.Parse(typeof(MPN), mi.m_strCateName);
			}
			catch
			{
				UnityEngine.Debug.LogWarning("カテゴリがありません。" + mi.m_strCateName);
				mi.m_mpn = MPN.null_mpn;
			}
			if (mpn != MPN.null_mpn)
			{
				mi.m_eColorSetMPN = mpn;
				if (!string.IsNullOrEmpty(text6))
				{
					mi.m_strMenuNameInColorSet = text6;
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				byte[] data = dictionary[text2];
				mi.m_texIcon = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				mi.m_texIcon.LoadImage(data);
			}
			mi.m_fPriority = 999f;
			using (StringReader stringReader = new StringReader(s))
			{
				string empty = string.Empty;
				string text7;
				while ((text7 = stringReader.ReadLine()) != null)
				{
					string[] array = text7.Split(new char[]
					{
					'\t',
					' '
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array[0] == "テクスチャ変更")
					{
						MaidParts.PARTS_COLOR pcMultiColorID = MaidParts.PARTS_COLOR.NONE;
						if (array.Length == 6)
						{
							string text8 = array[5];
							try
							{
								pcMultiColorID = (MaidParts.PARTS_COLOR)Enum.Parse(typeof(MaidParts.PARTS_COLOR), text8.ToUpper());
							}
							catch
							{
								NDebug.Assert("無限色IDがありません。" + text8, false);
							}
							mi.m_pcMultiColorID = pcMultiColorID;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 둘다 사용
		/// </summary>
		/// <param name="mi"></param>
		/// <returns></returns>
		private static string GetParentMenuFileName(SMenuItem mi)
		{
			if ((MPN.wear > mi.m_mpn || mi.m_mpn > MPN.onepiece) && (MPN.set_maidwear > mi.m_mpn || mi.m_mpn > MPN.set_body))
			{
				return string.Empty;
			}
			string text = mi.m_strMenuFileName;
			string result = string.Empty;
			text = text.ToLower();
			int num = text.IndexOf("_z");
			if (0 < num)
			{
				int num2 = text.IndexOf('_', num + 1);
				if (num2 == -1)
				{
					num2 = text.IndexOf('.', num + 1);
				}
				if (0 < num2 - num && 0 < text.Length - num2)
				{
					result = text.Substring(0, num) + text.Substring(num2, text.Length - num2);
				}
			}
			return result;
		}

		/// <summary>
		/// 공식모드에서 사용
		/// </summary>
		/// <param name="mi"></param>
		/// <param name="menuDataBaseIndex"></param>
		public static void ReadMenuItemDataFromNative(SMenuItem mi, int menuDataBaseIndex)
		{
			MenuDataBase menuDataBase = GameMain.Instance.MenuDataBase;
			menuDataBase.SetIndex(menuDataBaseIndex);
			mi.m_strMenuName = menuDataBase.GetMenuName();
			//mi.m_strInfo = menuDataBase.GetItemInfoText();
			mi.m_mpn = (MPN)menuDataBase.GetCategoryMpn();
			mi.m_strCateName = menuDataBase.GetCategoryMpnText();
			mi.m_eColorSetMPN = (MPN)menuDataBase.GetColorSetMpn();
			mi.m_strMenuNameInColorSet = menuDataBase.GetMenuNameInColorSet();
			mi.m_pcMultiColorID = (MaidParts.PARTS_COLOR)menuDataBase.GetMultiColorId();
			//mi.m_boDelOnly = menuDataBase.GetBoDelOnly();
			mi.m_fPriority = menuDataBase.GetPriority();
			mi.m_bMan = menuDataBase.GetIsMan();
			//mi.m_collabo = menuDataBase.GetIsCollabo();
			//mi.m_bOld = (menuDataBase.GetVersion() < 2000);
			string iconS = menuDataBase.GetIconS();
			if (!string.IsNullOrEmpty(iconS) && GameUty.FileSystem.IsExistentFile(iconS))
			{
				//if (SceneEdit.Instance != null)
				//logger.LogDebug("ImportCM.CreateTexture : " + mi.m_nMenuFileRID + " , " +  mi.m_strMenuName + " , " + iconS );
				editItemTextureCache.PreLoadRegister(mi.m_nMenuFileRID, iconS);
				// 이시점엔 아래 null 이라서 CreateTexture 발생
				//if (SceneEdit.Instance != null)
				//{
				//	SceneEdit.Instance.editItemTextureCache.PreLoadRegister(mi.m_nMenuFileRID, iconS);
				//}
				//else
				//{
				//try
				//{
				//	mi.m_texIcon = ImportCM.CreateTexture(iconS);
				//}
				//catch (Exception)
				//{
				//	logger.LogError("ReadMenuItemDataFromNative Error : " + mi.m_strMenuName + " , " + iconS);
				//}
				//}
			}
		}


		public static List<SceneEdit.SCategory> m_listCategory = new List<SceneEdit.SCategory>();

		public static HashSet<MPN> enabledMpns = new HashSet<MPN>();

		//TODO
		/*
		public static bool AddMenuItemToList(SMenuItem f_mi)
		{
			MPN mpn = f_mi.m_mpn;
			if (f_mi.m_mpn == MPN.null_mpn)
			{
				UnityEngine.Debug.LogWarning("カテゴリがnullの為、メニュー表示しません。" + f_mi.m_strCateName);
				return false;
			}
			SceneEditInfo.CCateNameType cateType;
			if (!string.IsNullOrEmpty(f_mi.m_strMenuFileName) && Product.type == Product.Type.JpAdult)
			{
				if (SceneEditInfo.m_listCollaboCategory.Contains(f_mi.m_strMenuFileName.ToLower()) || f_mi.m_collabo)
				{
					if (mpn != MPN.null_mpn && SceneEditInfo.m_dicPartsTypePair.TryGetValue(mpn, out cateType))
					{
						cateType.m_strBtnPartsTypeName = "コラボ";
						cateType.m_ePartsType = "set_collabo";
					}
				}
				else if (mpn != MPN.null_mpn && SceneEditInfo.m_dicPartsTypePair.TryGetValue(mpn, out cateType) && cateType.m_strBtnPartsTypeName == "コラボ" && cateType.m_ePartsType == "set_collabo")
				{
					if (mpn == MPN.set_maidwear)
					{
						cateType.m_strBtnPartsTypeName = "メイド服";
						cateType.m_ePartsType = "set_maidwear";
					}
					else if (mpn == MPN.set_mywear)
					{
						cateType.m_strBtnPartsTypeName = "コスチューム";
						cateType.m_ePartsType = "set_mywear";
					}
					else if (mpn == MPN.set_underwear)
					{

						cateType.m_strBtnPartsTypeName = "下着";
						cateType.m_ePartsType = "set_underwear";
					}
				}
			}
			if (mpn != MPN.null_mpn && SceneEditInfo.m_dicPartsTypePair.TryGetValue(mpn, out cateType))
			{
				if (cateType.m_eType == SceneEditInfo.CCateNameType.EType.Item || cateType.m_eType == SceneEditInfo.CCateNameType.EType.Set || cateType.m_eType == SceneEditInfo.CCateNameType.EType.Slider)
				{
					SCategory scategory = m_listCategory.Find((SCategory c) => c.m_eCategory == cateType.m_eMenuCate);
					SPartsType spartsType = scategory.m_listPartsType.Find((SPartsType p) => p.m_ePartsType == cateType.m_ePartsType);
					if (spartsType == null)
					{
						string f_strPartsTypeName = string.Empty;
						if (cateType.m_eType == SceneEditInfo.CCateNameType.EType.Slider)
						{
							if (!SceneEditInfo.m_dicSliderPartsTypeBtnName.TryGetValue(cateType.m_ePartsType, out f_strPartsTypeName))
							{
								NDebug.Assert("スライダー群に対するパーツタイプ名がみつかりませんでした。", false);
							}
						}
						else
						{
							f_strPartsTypeName = cateType.m_strBtnPartsTypeName;
						}
						spartsType = new SceneEdit.SPartsType(cateType.m_eType, mpn, f_strPartsTypeName, cateType.m_ePartsType, cateType.m_requestNewFace, cateType.m_requestFBFace);
						if (enabledMpns.Count == 0)
						{
							spartsType.m_isEnabled = true;
						}
						else
						{
							spartsType.m_isEnabled = enabledMpns.Contains(spartsType.m_mpn);
						}
						scategory.m_listPartsType.Add(spartsType);
					}
					f_mi.m_ParentPartsType = spartsType;
					if (cateType.m_eType == SceneEditInfo.CCateNameType.EType.Slider)
					{
						f_mi.m_fPriority = (float)cateType.m_nIdx;
					}
					spartsType.m_listMenu.Add(f_mi);
				}
				else if (cateType.m_eType == SceneEditInfo.CCateNameType.EType.Color)
				{
					f_mi.m_bColor = true;
					if (!this.m_dicColor.ContainsKey(f_mi.m_mpn))
					{
						this.m_dicColor.Add(f_mi.m_mpn, new List<SMenuItem>());
					}
					this.m_dicColor[f_mi.m_mpn].Add(f_mi);
				}
			}
			return true;
		}
		*/

		//TODO
		/*
		//private IEnumerator FixedInitMenu(List<SceneEdit.SMenuItem> menuList, Dictionary<int, SceneEdit.SMenuItem> menuRidDic, Dictionary<int, List<int>> menuGroupMemberDic)
		public static void FixedInitMenu(List<SMenuItem> menuList, Dictionary<int, SMenuItem> menuRidDic, Dictionary<int, List<int>> menuGroupMemberDic)
		{
			float time = Time.realtimeSinceStartup;
			string[] modfiles = Menu.GetModFiles();
			if (modfiles != null)
			{
				foreach (string strFileName in modfiles)
				{
					SMenuItem mi2 = new SMenuItem();
					if (InitModMenuItemScript(mi2, strFileName))
					{
						AddMenuItemToList(mi2);
						menuList.Add(mi2);
						if (!menuRidDic.ContainsKey(mi2.m_nMenuFileRID))
						{
							menuRidDic.Add(mi2.m_nMenuFileRID, mi2);
						}
						else
						{
							menuRidDic[mi2.m_nMenuFileRID] = mi2;
						}
						string parentMenuName = GetParentMenuFileName(mi2);
						if (!string.IsNullOrEmpty(parentMenuName))
						{
							int hashCode = parentMenuName.GetHashCode();
							if (!menuGroupMemberDic.ContainsKey(hashCode))
							{
								menuGroupMemberDic.Add(hashCode, new List<int>());
							}
							menuGroupMemberDic[hashCode].Add(mi2.m_strMenuFileName.ToLower().GetHashCode());
						}
						else if (mi2.m_strCateName.IndexOf("set_") != -1 && mi2.m_strMenuFileName.IndexOf("_del") == -1)
						{
							mi2.m_bGroupLeader = true;
							mi2.m_listMember = new List<SMenuItem>();
							mi2.m_listMember.Add(mi2);
						}
						if (0.5f < Time.realtimeSinceStartup - time)
						{
							yield return null;
							time = Time.realtimeSinceStartup;
						}
					}
				}
			}
			foreach (KeyValuePair<int, List<int>> keyValuePair in menuGroupMemberDic)
			{
				if (menuRidDic.ContainsKey(keyValuePair.Key) && keyValuePair.Value.Count >= 1)
				{
					SMenuItem smenuItem = menuRidDic[keyValuePair.Key];
					smenuItem.m_bGroupLeader = true;
					smenuItem.m_listMember = new List<SMenuItem>();
					smenuItem.m_listMember.Add(smenuItem);
					for (int n = 0; n < keyValuePair.Value.Count; n++)
					{
						smenuItem.m_listMember.Add(menuRidDic[keyValuePair.Value[n]]);
						smenuItem.m_listMember[smenuItem.m_listMember.Count - 1].m_bMember = true;
						smenuItem.m_listMember[smenuItem.m_listMember.Count - 1].m_leaderMenu = smenuItem;
					}
					smenuItem.m_listMember.Sort(delegate (SMenuItem x, SMenuItem y)
					{
						if (x.m_fPriority == y.m_fPriority)
						{
							return 0;
						}
						if (x.m_fPriority < y.m_fPriority)
						{
							return -1;
						}
						if (x.m_fPriority > y.m_fPriority)
						{
							return 1;
						}
						return 0;
					});
					smenuItem.m_listMember.Sort((SMenuItem x, SMenuItem y) => x.m_strMenuFileName.CompareTo(y.m_strMenuFileName));
				}
			}
			foreach (KeyValuePair<MPN, SceneEditInfo.CCateNameType> keyValuePair2 in SceneEditInfo.m_dicPartsTypePair)
			{
				if (keyValuePair2.Value.m_eType == SceneEditInfo.CCateNameType.EType.Slider)
				{
					AddMenuItemToList(new SMenuItem
					{
						m_mpn = keyValuePair2.Key,
						//m_nSliderValue = 500,
						m_strCateName = keyValuePair2.Key.ToString(),
						m_strMenuName = keyValuePair2.Value.m_strBtnPartsTypeName,
						//m_requestNewFace = keyValuePair2.Value.m_requestNewFace,
						//m_requestFBFace = keyValuePair2.Value.m_requestFBFace
					});
				}
			}
			for (int nM = 0; nM < menuList.Count; nM++)
			{
				SMenuItem mi = menuList[nM];
				if (SceneEditInfo.m_dicPartsTypePair.ContainsKey(mi.m_eColorSetMPN))
				{
					if (mi.m_eColorSetMPN != MPN.null_mpn)
					{
						if (mi.m_strMenuNameInColorSet != null)
						{
							mi.m_strMenuNameInColorSet = mi.m_strMenuNameInColorSet.Replace("*", ".*");
							mi.m_listColorSet = m_dicColor[mi.m_eColorSetMPN].FindAll((SMenuItem i) => new Regex(mi.m_strMenuNameInColorSet).IsMatch(i.m_strMenuFileName));
						}
						else
						{
							mi.m_listColorSet = m_dicColor[mi.m_eColorSetMPN];
						}
					}
					if (0.5f < Time.realtimeSinceStartup - time)
					{
						yield return null;
						time = Time.realtimeSinceStartup;
					}
				}
			}
			for (int j = 0; j < m_listCategory.Count; j++)
			{
				m_listCategory[j].SortPartsType();
			}
			for (int k = 0; k < m_listCategory.Count; k++)
			{
				m_listCategory[k].SortItem();
			}
			foreach (SceneEdit.SCategory scategory in m_listCategory)
			{
				if (scategory.m_eCategory == SceneEditInfo.EMenuCategory.プリセット || scategory.m_eCategory == SceneEditInfo.EMenuCategory.ランダム || scategory.m_eCategory == SceneEditInfo.EMenuCategory.プロフィ\u30FCル || scategory.m_eCategory == SceneEditInfo.EMenuCategory.着衣設定)
				{
					scategory.m_isEnabled = true;
				}
				else
				{
					scategory.m_isEnabled = false;
					foreach (SceneEdit.SPartsType spartsType in scategory.m_listPartsType)
					{
						if (spartsType.m_isEnabled)
						{
							scategory.m_isEnabled = true;
							break;
						}
					}
				}
			}
			if (modeType == SceneEdit.ModeType.CostumeEdit)
			{
				SceneEditInfo.EMenuCategory[] array = new SceneEditInfo.EMenuCategory[]
				{
				SceneEditInfo.EMenuCategory.セット,
				SceneEditInfo.EMenuCategory.プリセット,
				SceneEditInfo.EMenuCategory.ランダム,
				SceneEditInfo.EMenuCategory.プロフィ\u30FCル
				};
				SceneEditInfo.EMenuCategory[] array2 = array;
				for (int l = 0; l < array2.Length; l++)
				{
					SceneEditInfo.EMenuCategory cate = array2[l];
					m_listCategory.Find((SceneEdit.SCategory c) => c.m_eCategory == cate).m_isEnabled = false;
				}
			}
			else if (maid.status.heroineType == HeroineType.Sub || maid.boNPC)
			{
				SceneEditInfo.EMenuCategory[] array3 = new SceneEditInfo.EMenuCategory[]
				{
				SceneEditInfo.EMenuCategory.プロフィ\u30FCル
				};
				SceneEditInfo.EMenuCategory[] array4 = array3;
				for (int m = 0; m < array4.Length; m++)
				{
					SceneEditInfo.EMenuCategory cate = array4[m];
					m_listCategory.Find((SCategory c) => c.m_eCategory == cate).m_isEnabled = false;
				}
			}
			UpdatePanel_Category();
			yield break;
		}
		*/

	}
}
