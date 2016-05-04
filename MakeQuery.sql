select catid,name,total,no from
(
select catid,name,count(id) as total from 
(select catid,name,id from (select id as catid,name from cat1) as A left join 
(select FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.id from documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'luật đất đai')  AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] and KEY_TBL.RANK>0) as B on A.catid=B.cat1_id ) as C 
group by catid,name
) as total left join (select id,no from cat1) as total2 on total.catid=total2.id order by no desc, name