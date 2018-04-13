select top 10 * from ReportMaster order by createdon desc

;WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY ServiceId ORDER BY createdon DESC) AS rn
   FROM ReportMaster
)
SELECT *
FROM cte
WHERE rn = 1

