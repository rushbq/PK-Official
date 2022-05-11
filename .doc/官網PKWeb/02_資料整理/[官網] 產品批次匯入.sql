/* �Ѽƫŧi */
DECLARE @NewID AS INT, @AreaCode AS INT, @myModel AS NVARCHAR(40)
SET @AreaCode = 1  --/1 = Global, 2 = �x�W, 3 = ����

/* ��ƨӷ�(�j��) */
DECLARE cur CURSOR LOCAL FOR 
 SELECT Model_No FROM _Tmp_ModelNo

BEGIN
	OPEN cur
	FETCH NEXT FROM cur INTO @myModel  --Ū���Ĥ@�����
	WHILE @@FETCH_STATUS = 0
       BEGIN
		/* ��ƳB�z Start */
		
		 --// �ˬd�O�_������
		 IF (SELECT COUNT(*) FROM Prod WHERE (AreaCode = @AreaCode) AND (Model_No = @myModel)) = 0
		 BEGIN

			--//���o�s�s��
			 SET @NewID = (
			  SELECT ISNULL(MAX(Prod_ID) ,0) + 1 FROM Prod 
			 );

			--//���~�D��
			  INSERT INTO Prod( 
			   Prod_ID, AreaCode, Model_No, StartTime, EndTime, Display, IsNew, Sort
			   , Create_Who, Create_Time
			  ) VALUES ( 
			   @NewID, @AreaCode, @myModel, '2015-01-01 00:00', '2025-12-31 00:00', 'Y', 'N', 999
			   , '{ca3f4d99-57df-4dc3-b309-93feff622dcf}', '2015-01-01 00:00'
			  );

			--//�{�ҲŸ�
			 INSERT INTO Prod_Rel_CertIcon( 
			  Prod_ID, Pic_ID
			 ) 
			 SELECT @NewID, Icon_Pics.Pic_ID
			 FROM [ProductCenter].dbo.Prod_Certification Base WITH (NOLOCK) 
			  INNER JOIN [ProductCenter].dbo.Prod_Certification_Detail Sub WITH (NOLOCK) ON Base.Cert_ID = Sub.Cert_ID 
			  INNER JOIN [ProductCenter].dbo.Icon_Rel_Certification Rel WITH (NOLOCK) ON Rel.Cert_ID = Sub.Cert_ID AND Rel.Detail_ID = Sub.Detail_ID 
			  INNER JOIN [ProductCenter].dbo.Icon_Pics WITH (NOLOCK) ON Rel.Pic_ID = Icon_Pics.Pic_ID 
			  INNER JOIN [ProductCenter].dbo.Icon WITH (NOLOCK) ON Icon_Pics.Icon_ID = Icon.Icon_ID 
			 WHERE (Base.Model_No = @myModel) AND (Icon.Display = 'Y')
			 GROUP BY Icon_Pics.Pic_ID, Icon_Pics.Pic_File 

		 END
		
		/* ��ƳB�z End */
       FETCH NEXT FROM cur INTO @myModel   --Ū���U�@�����
       END
     CLOSE cur          --����cur
     DEALLOCATE cur     --����cur

END


