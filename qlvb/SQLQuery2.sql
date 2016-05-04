use qlvb
SELECT top 100 
             FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id, FT_TBL.views, 
			 RANK=CASE FT_TBL.cat2_id
         WHEN 7 THEN KEY_TBL.RANK*7
         WHEN 18 THEN KEY_TBL.RANK*6
         WHEN 15 THEN KEY_TBL.RANK*5
		 WHEN 5 THEN KEY_TBL.RANK*4
         WHEN 23 THEN KEY_TBL.RANK*3
		 WHEN 6 THEN KEY_TBL.RANK*2
         ELSE KEY_TBL.RANK
      END FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'luật đất đai') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] 
			 order by Rank Desc

			 update documents set auto_des=+code+' '+name+' '+code+' '+name+' '+code+' '+name+' '+Cast(auto_des as nvarchar(4000)) where id=44

			 SELECT top 100 
             FT_TBL.code as name FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'dinh muc kinh te') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] 
			 where Rank>0 order by Rank Desc

			  SELECT top 100 
             FT_TBL.name,KEY_TBL.RANK FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'do ve ban do') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] 
			 order by Rank Desc

			 select catid,name,count(id) as total from
			 (select catid,name,id from
			 (select id as catid,name from cat1) as A left join
			 (select cat1_id,id from documents where cat1_id=1) as B on A.catid=B.cat1_id
			 ) as C group by catid,name