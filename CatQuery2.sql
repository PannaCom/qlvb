
  use qlvb
  select id,code,name,cat1_id,cat1,cat2_id,cat2,cat4_id,cat4,views,no from
  (select id,code,name,cat1_id,cat2_id,cat4_id,views from documents) as A left join
  (select name as cat1,id as idcat1 from cat1) as B on A.cat1_id=B.idcat1 left join
  (select name as cat2,id as idcat2,no from cat2) as C on A.cat2_id=C.idcat2 left join
  (select name as cat4,id as idcat4 from cat4) as D on A.cat4_id=D.idcat4 where 1=1  
  order by no desc

  select catid,name,count(*) as total from
  (
  select cat1_id as catid,cat1 as name from
  (select id,code,name,cat1_id,cat2_id,cat4_id,views from documents) as A left join
  (select name as cat1,id as idcat1 from cat1) as B on A.cat1_id=B.idcat1 left join
  (select name as cat2,id as idcat2,no from cat2) as C on A.cat2_id=C.idcat2 left join
  (select name as cat4,id as idcat4 from cat4) as D on A.cat4_id=D.idcat4  where cat1_id=6 and cat2_id=7
  ) as total group by catid,name

  