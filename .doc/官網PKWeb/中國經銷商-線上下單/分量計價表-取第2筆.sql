--宣告今天的日期
DECLARE @TODAY nChar(8)
SET @TODAY = CONVERT(nChar(8),GETDATE(),112)

DECLARE @CustID VARCHAR(50)
SET @CustID = 'C010001';

/* ref: https://dotblogs.com.tw/fish_lin/2017/04/24/115821 */

;
WITH TblCust AS (
	-- 客戶(Cust) --
	SELECT RTRIM(MA001) AS CustID, MA002 AS CustName
	FROM  [ProUnion].[dbo].[COPMA] WITH (NOLOCK)
	--WHERE (MA001 = @CustID)
	WHERE (MA001 IN ('C591013','C574011','C010002'))
),
TblA AS (
	-- 本體(A) -- COPMB依生效日排序,設定序號,並關聯客戶(Cust)
	SELECT 
		RTRIM(myBase.MB001) AS CustID --AS [客戶代號]
		,RTRIM(myBase.MB002) AS ModelNo --AS [品號]
		,myBase.MB004 AS Currency --AS [幣別]
		,myBase.MB007 AS IsSpQty --AS [分量計價]
		,myBase.MB008 AS UnitPrice --AS [單價]
		,myBase.MB010 AS LastSaleDay --AS [上次銷貨日]
		,myBase.MB013 AS WithTax --AS [含稅]
		,myBase.MB017 AS ValidDate --AS [生效日]
		,myBase.MB018 AS InValidDate --AS [失效日]
		,myBase.MB019 AS TransTerm --AS [交易條件]
		,myBase.MB009 AS CheckPriceDate --AS [核價日]
	 , RANK() OVER (
		PARTITION BY myBase.MB001, myBase.MB002, myBase.MB003, myBase.MB004, myBase.MB019 ORDER BY myBase.MB017 DESC
	) AS ValidDateRank
	FROM  [ProUnion].[dbo].[COPMB] AS myBase WITH (NOLOCK)
	JOIN TblCust ON TblCust.CustID = myBase.MB001
	--//條件:含稅, 生效日小於今日, 客戶
	WHERE (myBase.MB013 = 'Y') AND (myBase.MB017 <= @TODAY)
),
TblB AS (
	-- (B) -- COPMC計價檔 關聯已整理過的(GP-A)
	SELECT
		ISNULL(TbQuote.MC005, 1) AS spQty  --數量(分量計價檔)
		, RTRIM(MC001) AS CustID
		, RTRIM(MC002) AS ModelNo
		, ISNULL(TbQuote.MC006, TblA.UnitPrice) AS SpQtyPrice	--價格(分量計價無資料則抓售價)
		, RANK() OVER (
			PARTITION BY TbQuote.MC001, TbQuote.MC002, TbQuote.MC004
			ORDER BY TbQuote.MC002 ASC, TbQuote.MC005 ASC
		) AS spRank
	FROM [ProUnion].[dbo].[COPMC] AS TbQuote WITH (NOLOCK)
	--//關聯條件:客戶代號/品號/幣別/生效日
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
