
select name,no,count(*) as total from
(select id,name,no from cat1) as A left join
(select cat1_id from documents) as B on A.id=B.cat1_id
group by name,no order by no desc


select name,no,count(*) as total from
(select id,name,no from cat2) as A left join
(select cat2_id from documents) as B on A.id=B.cat2_id
group by name,no order by no desc