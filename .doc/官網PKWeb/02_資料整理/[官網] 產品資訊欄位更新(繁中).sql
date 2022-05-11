/*
  [前置作業]
   1) 先把舊官網資料匯出(EC.dbo.items)
   2) 使用產生指令碼的方式，儲存sql檔
   3) 微調sql檔，於ProductCenter匯入資料並命名為[_Tmp_Items]
*/
USE [ProductCenter]

/*
  [參數宣告]
  @myModel = 品號, @myLang = 語系
  @dataFeature = 特性資料, @dataSpec = 規格資料
*/
DECLARE @myModel AS NVARCHAR(40), @myLang AS VARCHAR(6)
		, @dataFeature AS NVARCHAR(MAX), @dataSpec AS NVARCHAR(MAX)

/* 設定參數 */
SET @myLang = 'zh-TW'


/* 資料來源(品號迴圈) */
DECLARE cur CURSOR LOCAL FOR 
	/*
	  取得品號:ProductCenter, 特性(info2)/規格(info4) 欄位皆為空的品號
	*/
	SELECT Tbl.Model_No
	FROM(
		SELECT RTRIM(Base.Model_No) AS Model_No
		, CONVERT(NVARCHAR(max), Info.Info2) AS iFeature
		, CONVERT(NVARCHAR(max), Info.Info4) AS iSpec
		FROM Prod_Item Base
			LEFT JOIN Prod_Info Info ON Base.Model_No = Info.Model_No AND Info.Lang = @myLang
		WHERE (Base.Model_No IN (
			/* 新官網 PKWeb.dbo.Prod 有上線的品號 */
			SELECT Prod.Model_No FROM PKWeb.dbo.Prod WHERE (Prod.Display = 'Y')
			GROUP BY Prod.Model_No
		))
	) AS Tbl
	WHERE (iFeature IS NULL OR iFeature = '') AND (iSpec IS NULL OR iSpec = '') --AND Model_No = '103-132C' --//for test//--

BEGIN
	OPEN cur
	FETCH NEXT FROM cur INTO @myModel  --讀取第一筆資料
	WHILE @@FETCH_STATUS = 0
       BEGIN
		/* 資料處理 Start */
		 --// 取得舊官網資料 EC.dbo.items (繁中欄位)
		 SET @dataFeature = (SELECT TOP 1 chn_feature FROM _Tmp_Items WHERE (name = @myModel))
		 SET @dataSpec = (SELECT TOP 1 chn_spec FROM _Tmp_Items WHERE (name = @myModel))
		
		 --// 檢查Prod_Info是否有資料
		 IF (SELECT COUNT(*) FROM Prod_Info WHERE (Lang = @myLang) AND (Model_No = @myModel)) = 0
		 BEGIN
			--[沒資料=INSERT]
			  INSERT INTO Prod_Info( 
			   Model_No, Lang, Info2, Info4, Create_Who, Create_Time
			  ) VALUES ( 
			   @myModel, @myLang, @dataFeature, @dataSpec, '10255', '2015-04-29 00:00'
			  );
		 END
		 
		ELSE
		 
		 BEGIN
			--[有資料=UPDATE]
			UPDATE Prod_Info
			SET Info2 = @dataFeature, Info4 = @dataSpec
			WHERE (Model_No = @myModel) AND (Lang = @myLang)
		 END
		 
		/* 資料處理 End */
       FETCH NEXT FROM cur INTO @myModel   --讀取下一筆資料
       END
     CLOSE cur          --關閉cur
     DEALLOCATE cur     --釋放cur

END


