--select * from product_master

--select * from product_detail


--select pd.pm_id,count(*) Qty, pm.pm_name from product_detail pd, product_master pm 
--where pd.pm_id = pm.pm_id
--group by pd.pm_id, pm.pm_name 



--select pm.pm_id product_id, pm.pm_name product, count(pd.pm_id) Qty from product_master pm
--left join
--product_detail pd on pd.pm_id = pm.pm_id group by pd.pm_id, pm.pm_id, pm.pm_name