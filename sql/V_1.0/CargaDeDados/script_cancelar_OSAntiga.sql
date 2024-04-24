update tbOSPadrao
	set st_status_os = 4
where
	(ST_STATUS_OS = 1 or ST_STATUS_OS = 2)
	and (dt_data_os < '2021-12-01 00:00:00.000' or DT_DATA_OS is null)


update tbVisitaPadrao
	set st_status_Visita = 5
where
	(st_status_Visita = 1 or st_status_Visita = 2 or st_status_Visita = 3)
	and (DT_DATA_Visita < '2021-12-01 00:00:00.000' or DT_DATA_Visita is null)