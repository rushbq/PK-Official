--�ŧi���Ѫ����
DECLARE @TODAY nChar(8)
SET @TODAY = CONVERT(nChar(8),GETDATE(),112)

DECLARE @CustID VARCHAR(50)
SET @CustID = 'C010001';

/* ref: https://dotblogs.com.tw/fish_lin/2017/04/24/115821 */

;
WITH TblCust AS (
	-- �Ȥ�(Cust) --
	SELECT RTRIM(MA001) AS CustID, MA002 AS CustName
	FROM  [ProUnion].[dbo].[COPMA] WITH (NOLOCK)
	--WHERE (MA001 = @CustID)
	WHERE (MA001 IN ('C591013','C574011','C010002'))
),
TblA AS (
	-- ����(A) -- COPMB�̥ͮĤ�Ƨ�,�]�w�Ǹ�,�����p�Ȥ�(Cust)
	SELECT 
		RTRIM(myBase.MB001) AS CustID --AS [�Ȥ�N��]
		,RTRIM(myBase.MB002) AS ModelNo --AS [�~��]
		,myBase.MB004 AS Currency --AS [���O]
		,myBase.MB007 AS IsSpQty --AS [���q�p��]
		,myBase.MB008 AS UnitPrice --AS [���]
		,myBase.MB010 AS LastSaleDay --AS [�W���P�f��]
		,myBase.MB013 AS WithTax --AS [�t�|]
		,myBase.MB017 AS ValidDate --AS [�ͮĤ�]
		,myBase.MB018 AS InValidDate --AS [���Ĥ�]
		,myBase.MB019 AS TransTerm --AS [�������]
		,myBase.MB009 AS CheckPriceDate --AS [�ֻ���]
	 , RANK() OVER (
		PARTITION BY myBase.MB001, myBase.MB002, myBase.MB003, myBase.MB004, myBase.MB019 ORDER BY myBase.MB017 DESC
	) AS ValidDateRank
	FROM  [ProUnion].[dbo].[COPMB] AS myBase WITH (NOLOCK)
	JOIN TblCust ON TblCust.CustID = myBase.MB001
	--//����:�t�|, �ͮĤ�p�󤵤�, �Ȥ�
	WHERE (myBase.MB013 = 'Y') AND (myBase.MB017 <= @TODAY)
),
TblB AS (
	-- (B) -- COPMC�p���� ���p�w��z�L��(GP-A)
	SELECT
		ISNULL(TbQuote.MC005, 1) AS spQty  --�ƶq(���q�p����)
		, RTRIM(MC001) AS CustID
		, RTRIM(MC002) AS ModelNo
		, ISNULL(TbQuote.MC006, TblA.UnitPrice) AS SpQtyPrice	--����(���q�p���L��ƫh����)
		, RANK() OVER (
			PARTITION BY TbQuote.MC001, TbQuote.MC002, TbQuote.MC004
			ORDER BY TbQuote.MC002 ASC, TbQuote.MC005 ASC
		) AS spRank
	FROM [ProUnion].[dbo].[COPMC] AS TbQuote WITH (NOLOCK)
	--//���p����:�Ȥ�N��/�~��/���O/�ͮĤ�
	JOIN TblA ON TblA.CustID = TbQuote.MC001
	 AND TblA.ModelNo = TbQuote.MC002 AND TblA.Currency = TbQuote.MC004
	 AND TblA.ValidDate = TbQuote.MC009
	 AND TblA.ValidDateRank = 1  --//Rank=1
)
SELECT RTRIM(BaseProd.MB001) ModelNo, BaseProd.MB201 InBoxQty, BaseProd.MB051 DefaultPrice
, TblB.CustID, ISNULL(TblB.spQty, 1) AS minQty
FROM [ProUnion].[dbo].[INVMB] BaseProd WITH (NOLOCK)
 LEFT JOIN TblB ON BaseProd.MB001 = TblB.ModelNo AND TblB.spRank = 2

ORDER BY BaseProd.MB001
