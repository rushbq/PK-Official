/*
  [�e�m�@�~]
   1) �����©x����ƶץX(EC.dbo.items)
   2) �ϥβ��ͫ��O�X���覡�A�x�ssql��
   3) �L��sql�ɡA��ProductCenter�פJ��ƨéR�W��[_Tmp_Items]
*/
USE [ProductCenter]

/*
  [�Ѽƫŧi]
  @myModel = �~��, @myLang = �y�t
  @dataFeature = �S�ʸ��, @dataSpec = �W����
*/
DECLARE @myModel AS NVARCHAR(40), @myLang AS VARCHAR(6)
		, @dataFeature AS NVARCHAR(MAX), @dataSpec AS NVARCHAR(MAX)

/* �]�w�Ѽ� */
SET @myLang = 'zh-TW'


/* ��ƨӷ�(�~���j��) */
DECLARE cur CURSOR LOCAL FOR 
	/*
	  ���o�~��:ProductCenter, �S��(info2)/�W��(info4) ���Ҭ��Ū��~��
	*/
	SELECT Tbl.Model_No
	FROM(
		SELECT RTRIM(Base.Model_No) AS Model_No
		, CONVERT(NVARCHAR(max), Info.Info2) AS iFeature
		, CONVERT(NVARCHAR(max), Info.Info4) AS iSpec
		FROM Prod_Item Base
			LEFT JOIN Prod_Info Info ON Base.Model_No = Info.Model_No AND Info.Lang = @myLang
		WHERE (Base.Model_No IN (
			/* �s�x�� PKWeb.dbo.Prod ���W�u���~�� */
			SELECT Prod.Model_No FROM PKWeb.dbo.Prod WHERE (Prod.Display = 'Y')
			GROUP BY Prod.Model_No
		))
	) AS Tbl
	WHERE (iFeature IS NULL OR iFeature = '') AND (iSpec IS NULL OR iSpec = '') --AND Model_No = '103-132C' --//for test//--

BEGIN
	OPEN cur
	FETCH NEXT FROM cur INTO @myModel  --Ū���Ĥ@�����
	WHILE @@FETCH_STATUS = 0
       BEGIN
		/* ��ƳB�z Start */
		 --// ���o�©x����� EC.dbo.items (�c�����)
		 SET @dataFeature = (SELECT TOP 1 chn_feature FROM _Tmp_Items WHERE (name = @myModel))
		 SET @dataSpec = (SELECT TOP 1 chn_spec FROM _Tmp_Items WHERE (name = @myModel))
		
		 --// �ˬdProd_Info�O�_�����
		 IF (SELECT COUNT(*) FROM Prod_Info WHERE (Lang = @myLang) AND (Model_No = @myModel)) = 0
		 BEGIN
			--[�S���=INSERT]
			  INSERT INTO Prod_Info( 
			   Model_No, Lang, Info2, Info4, Create_Who, Create_Time
			  ) VALUES ( 
			   @myModel, @myLang, @dataFeature, @dataSpec, '10255', '2015-04-29 00:00'
			  );
		 END
		 
		ELSE
		 
		 BEGIN
			--[�����=UPDATE]
			UPDATE Prod_Info
			SET Info2 = @dataFeature, Info4 = @dataSpec
			WHERE (Model_No = @myModel) AND (Lang = @myLang)
		 END
		 
		/* ��ƳB�z End */
       FETCH NEXT FROM cur INTO @myModel   --Ū���U�@�����
       END
     CLOSE cur          --����cur
     DEALLOCATE cur     --����cur

END


