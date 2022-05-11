--取得ASIA 所屬國家
SELECT Base.AreaCode, Base.Country_Code, Sub.Country_Name
FROM Geocode_CountryCode Base
	INNER JOIN Geocode_CountryName Sub ON Base.Country_Code = Sub.Country_Code
WHERE (Base.AreaCode = 'A') AND (Sub.LangCode = 'zh-tw')
ORDER BY Sub.Country_Name