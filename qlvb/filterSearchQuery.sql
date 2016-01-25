 select catid,name,count(id) as total from
(select catid,name,id from 
            (select id as catid,name from cat1) as A left join 
            (select FT_TBL.cat1_id,FT_TBL.id from documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'71/')  AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY]) as B on A.catid=B.cat1_id
            and cat1_id=8) as C group by catid,name