--//�x�� - ���o�U�~���s����
DECLARE @myTable TABLE(myIP varchar(20))
INSERT @myTable 
	SELECT '192.168.0' UNION ALL 
	SELECT '192.168.1' UNION ALL 
	SELECT '192.168.2' UNION ALL 
	SELECT '192.168.3' UNION ALL 
	SELECT '172.16.40'

SELECT COUNT(*) AS '�s����', EventDesc
FROM Log_Event
WHERE (EventID = '1002') AND (FromIP <> 'local') AND LEFT(FromIP, 9) Not IN 
(SELECT myIP FROM @myTable)
GROUP BY EventDesc
ORDER BY 1 DESC


