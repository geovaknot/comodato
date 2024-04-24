GO
/****** Object:  StoredProcedure [dbo].[prcRelatorioVendasCompletosSelect]    Script Date: 24/04/2023 12:09:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--select * from tbOSPadrao order by id_OS desc

-- =============================================  
-- Author: Andre Farinelli/Paulo Rabelo
-- Create date: 26/08/2019  
-- Description: Seleção de dados de Analise de Vendas  
-- Completo.  
-- =============================================  
  
ALTER PROCEDURE [dbo].[prcRelatorioVendasCompletosSelect]  
  --declare
  @p_CD_LINHA_PRODUTO NUMERIC(6,0) = NULL,  
  @p_CD_CLIENTE  NUMERIC(6,0) = NULL,  
  @p_CD_VENDEDOR  NUMERIC(6,0) = NULL,  
  @p_CD_EXECUTIVO  NUMERIC(3,0) = NULL  
AS  
BEGIN  
  
 SET NOCOUNT ON  
    SET ANSI_WARNINGS OFF  
    SET FMTONLY OFF  
  
  
 -- Declaração de Variáveis  
 DECLARE @cdsErrorMessage NVARCHAR(4000),  
   @nidErrorSeverity INT,  
   @nidErrorState  INT  
  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
 BEGIN TRY  
  
 IF  OBJECT_ID('tempdb..#TB_FILTRO_VENDAS') IS NOT NULL  
 BEGIN  
  DROP TABLE #TB_FILTRO_VENDAS  
 END   
  
 SELECT CD_CLIENTE, CD_LINHA_PRODUTO  
 INTO #TB_FILTRO_VENDAS  
 FROM V_VENDAS_LINHA_ULT_12_ALL  
 WHERE (CD_CLIENTE = @p_CD_CLIENTE OR @p_CD_CLIENTE IS NULL)  
 AND (CD_LINHA_PRODUTO = @p_CD_LINHA_PRODUTO OR @p_CD_LINHA_PRODUTO IS NULL)  
 GROUP BY CD_CLIENTE, CD_LINHA_PRODUTO  
  
  
 -- Data For prcRelatorioCompletosSelect  
 Select  
  CLI.CD_CLIENTE,  
  CLI.NM_CLIENTE,  
  VND.CD_VENDEDOR,  
  VND.NM_VENDEDOR,  
  EXE.CD_EXECUTIVO,  
  EXE.NM_EXECUTIVO,  
  LIN.CD_LINHA_PRODUTO,  
  LIN.DS_LINHA_PRODUTO,  
  ATI.DS_MODELO,  
  dbo.fCountEQP(CLI.CD_CLIENTE,LIN.CD_LINHA_PRODUTO) AS QTD_EQUIP,  
  VL_CUSTO_ATIVO,
	CASE 
		WHEN ( VL_DEPREC_ULT_MES > 0 ) THEN 
			VL_CUSTO_ATIVO - VL_DEPREC_ULT_MES * DIFF
		ELSE
			0
	END as VL_RESIDUAL,    
	CASE 
		WHEN ( VL_DEPREC_ULT_MES > 0 ) THEN 
			(VL_DEPREC_ULT_MES * DIFF)
		ELSE
			-VL_DEPREC_TOTAL
	END as VL_DEPRECIACAO,    
  (VL_DEPREC_ULT_MES) AS ULT_DEP,  
  DATEDIFF(m,DATEADD(HH,-3,GETUTCDATE()),DATEADD(m,NR_MESES,DT_INICIO_DEPREC)) AS MESES_DEP,  
  ATI.CD_ATIVO_FIXO,  
  ATI.DT_NOTAFISCAL,  
  ATI.NR_NOTAFISCAL,  
  ATI.DT_DEVOLUCAO  
 From  
  V_VENDAS_LINHA_ULT_12_ALL VEN  
  INNER JOIN #TB_FILTRO_VENDAS FV ON FV.CD_CLIENTE = VEN.CD_CLIENTE AND FV.CD_LINHA_PRODUTO = VEN.CD_LINHA_PRODUTO  
  LEFT join TB_CLIENTE CLI on VEN.CD_CLIENTE = CLI.CD_CLIENTE  
  LEFT join TB_VENDEDOR VND on VND.CD_VENDEDOR = CLI.CD_VENDEDOR  
  LEFT Join TB_LINHA_PRODUTO LIN on VEN.CD_LINHA_PRODUTO = LIN.CD_LINHA_PRODUTO  
  LEFT join TB_EXECUTIVO EXE on CLI.CD_EXECUTIVO = EXE.CD_EXECUTIVO  
  LEFT Join (  
			   select  
				EQC.CD_CLIENTE,  
				EQC.CD_ATIVO_FIXO,  
				EQC.DT_NOTAFISCAL,  
				EQC.NR_NOTAFISCAL,  
				EQC.DT_DEVOLUCAO,  
				EQP.CD_LINHA_PRODUTO,  
				MOD.DS_MODELO,  
				DEP.VL_CUSTO_ATIVO,  
				DEP.VL_DEPREC_TOTAL,  
				DEP.VL_DEPREC_ULT_MES,  
				DEP.DT_INICIO_DEPREC,  
				DEP.NR_MESES,
				DEP.DIFF  
			   from  
				TB_ATIVO_CLIENTE EQC  
				LEFT JOIN TB_ATIVO_FIXO EQP on EQP.CD_ATIVO_FIXO = EQC.CD_ATIVO_FIXO  
				LEFT JOIN TB_MODELO MOD on EQP.CD_MODELO = MOD.CD_MODELO  
				LEFT JOIN V_DEPRECIACAO_ATIVO_CLIENTE_ULT_MES DEP on EQP.CD_ATIVO_FIXO = DEP.CD_ATIVO_FIXO  
				INNER JOIN TB_CLIENTE CLI1 on CLI1.CD_CLIENTE = EQC.CD_CLIENTE  
				INNER JOIN #TB_FILTRO_VENDAS FV ON FV.CD_CLIENTE = CLI1.CD_CLIENTE AND FV.CD_LINHA_PRODUTO = EQP.CD_LINHA_PRODUTO  
    
			   WHERE DT_DEVOLUCAO IS NULL  
				AND (CLI1.CD_EXECUTIVO  = @p_CD_EXECUTIVO  OR @p_CD_EXECUTIVO IS NULL)  
				AND (CLI1.CD_VENDEDOR  = @p_CD_VENDEDOR  OR @p_CD_VENDEDOR IS NULL)  
       ) ATI  
		on CLI.CD_CLIENTE = ATI.CD_CLIENTE and LIN.CD_LINHA_PRODUTO = ATI.CD_LINHA_PRODUTO  
  WHERE (CLI.CD_CLIENTE  = @p_CD_CLIENTE  OR @p_CD_CLIENTE IS NULL)  
	  AND (EXE.CD_EXECUTIVO  = @p_CD_EXECUTIVO  OR @p_CD_EXECUTIVO IS NULL)  
	  AND (VND.CD_VENDEDOR  = @p_CD_VENDEDOR  OR @p_CD_VENDEDOR IS NULL)  
	  AND (VND.FL_ATIVO = 'S')
	  AND (LIN.CD_LINHA_PRODUTO  = @p_CD_LINHA_PRODUTO  OR @p_CD_LINHA_PRODUTO IS NULL)  
	  and ( VEN.TOT_VENDAS<>0 OR dbo.fCountEQP(CLI.CD_CLIENTE,LIN.CD_LINHA_PRODUTO)>0 )  
  Order By 
	NM_CLIENTE, DS_LINHA_PRODUTO  
  
  
  
  -- Data for prcRelatorioCompletosSub1Select  
  Select
		VEA.CD_CLIENTE,
		VEA.CD_LINHA_PRODUTO,
		dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO) AS NR_EQUIPAMENTO,
		ISNULL(((VEA.QT_VENDAS_CV / 12) / NULLIF(dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO),0)),0) as fConsumo,
		(CASE WHEN COALESCE(EXC.VL_EXPECTATIVA,0) = 0 THEN LIN.VL_EXPECTATIVA_PADRAO ELSE EXC.VL_EXPECTATIVA END )  as EXPECTATIVA_CONSUMO,

		CASE WHEN COALESCE(EXC.VL_EXPECTATIVA,0) = 0 THEN
			ROUND(((ISNULL(((VEA.QT_VENDAS_CV / 12) / NULLIF(dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO),0)),0) / Convert(float, LIN.VL_EXPECTATIVA_PADRAO)) * 100.0),2)
		ELSE
			ROUND(((ISNULL(((VEA.QT_VENDAS_CV / 12) / NULLIF(dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO),0)),0) / Convert(float, EXC.VL_EXPECTATIVA)) * 100.0),2)
		END AS fAtingimento,

		CASE WHEN COALESCE(EXC.VL_EXPECTATIVA,0) = 0 THEN
			ROUND(100 - ((ISNULL(((VEA.QT_VENDAS_CV / 12) / NULLIF(dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO),0)),0) / Convert(float, LIN.VL_EXPECTATIVA_PADRAO)) * 100.0),2)
		ELSE
			ROUND(100 - ((ISNULL(((VEA.QT_VENDAS_CV / 12) / NULLIF(dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO),0)),0) / Convert(float, EXC.VL_EXPECTATIVA)) * 100.0),2)
		END AS fRestante,		
		
		IsNull(VEM.TOT_VENDAS_LINHA,0) as VL_VENDAS,
		VEA.TOT_VENDAS as TOT_VENDAS,
		COALESCE(CUM.CUSTO,0) as CUSTO_MES,
		COALESCE(CUA.CUSTO,0) as TOT_CUSTO,
		COALESCE(VEM.DEPCOM,0) as DEPCOM,
		COALESCE(VEA.DEPCOM,0) as TOT_DEPCOM,
		COALESCE(VEM.LESAFO,0) as LESAFO,
		COALESCE(VEA.LESAFO,0) as TOT_LESAFO,
		COALESCE(DEM.TOT_DEPRECIACAO,0) as DEPRECIACAO,
		COALESCE(DEA.TOT_DEPRECIACAO,0) as  TOT_DEPRECIACAO,
		--ULT_MES
		ISNULL((CASE WHEN VEM.TOT_VENDAS_LINHA <> 0 then
			(1 - (((COALESCE(MAM.TOT_PECAS,0) + COALESCE(MAM.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEM.TOT_DEPRECIACAO,0)) + (VEM.TOT_VENDAS_LINHA * COALESCE(CUM.CUSTO,0))) / VEM.TOT_VENDAS_LINHA) - COALESCE(VEA.DEPCOM,0))*100 --AS fGM,
		ELSE 0 END),0) AS fGM,
		ISNULL((CASE WHEN VEM.TOT_VENDAS_LINHA <> 0 then
			(1 - (((COALESCE(MAM.TOT_PECAS,0) + COALESCE(MAM.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEM.TOT_DEPRECIACAO,0)) + (VEM.TOT_VENDAS_LINHA * COALESCE(CUM.CUSTO,0))) / VEM.TOT_VENDAS_LINHA) - COALESCE(VEM.DEPCOM,0) - COALESCE(VEM.LESAFO,0))*100 
		ELSE 0 END),0) AS fOI,
		ISNULL((CASE WHEN VEM.TOT_VENDAS_LINHA <> 0 then
			(VEM.TOT_VENDAS_LINHA * (1 - (((COALESCE(MAM.TOT_PECAS,0) + COALESCE(MAM.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEM.TOT_DEPRECIACAO,0)) + (VEM.TOT_VENDAS_LINHA * COALESCE(CUM.CUSTO,0))) / VEM.TOT_VENDAS_LINHA) - COALESCE(VEM.DEPCOM,0) - COALESCE(VEM.LESAFO,0))) 
		ELSE 0 END),0) AS fOIR,
		--ULT_12
		ISNULL((CASE WHEN VEA.TOT_VENDAS <> 0 then	
			(1 - (((COALESCE(MAA.TOT_PECAS,0) + COALESCE(MAA.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEA.TOT_DEPRECIACAO,0)) + (VEA.TOT_VENDAS * COALESCE(CUA.CUSTO,0))) / VEA.TOT_VENDAS) - COALESCE(VEA.DEPCOM,0))*100 
		ELSE 0 END),0) AS fTotGM,
		ISNULL((CASE WHEN VEA.TOT_VENDAS <> 0 then	
			(1 - (((COALESCE(MAA.TOT_PECAS,0) + COALESCE(MAA.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEA.TOT_DEPRECIACAO,0)) + (VEA.TOT_VENDAS * COALESCE(CUA.CUSTO,0))) / VEA.TOT_VENDAS) - COALESCE(VEA.DEPCOM,0) - COALESCE(VEA.LESAFO,0))*100 
		ELSE 0 END),0) AS fTotOI,
		ISNULL((CASE WHEN VEA.TOT_VENDAS <> 0 then	
			(VEA.TOT_VENDAS * (1 - (((COALESCE(MAA.TOT_PECAS,0) + COALESCE(MAA.TOT_MAO_OBRA,0)) + ABS(COALESCE(DEA.TOT_DEPRECIACAO,0)) + (VEA.TOT_VENDAS * COALESCE(CUA.CUSTO,0))) / VEA.TOT_VENDAS) - COALESCE(VEA.DEPCOM,0) - COALESCE(VEA.LESAFO,0))) 
		ELSE 0 END),0) AS fOIMDR
	From
		V_VENDAS_LINHA_ULT_12_ALL VEA
		Inner Join
		TB_LINHA_PRODUTO LIN on LIN.CD_LINHA_PRODUTO = VEA.CD_LINHA_PRODUTO
		Inner Join
		TB_CLIENTE CLI on CLI.CD_CLIENTE = VEA.CD_CLIENTE
		Inner Join
		TB_VENDEDOR VND on VND.CD_VENDEDOR = CLI.CD_VENDEDOR
		LEFT JOIN
		TB_EXECUTIVO EXE on CLI.CD_EXECUTIVO = EXE.CD_EXECUTIVO
		Left join
		TB_EXPECTATIVA_CLIENTE EXC on VEA.CD_CLIENTE = EXC.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = EXC.CD_LINHA_PRODUTO
		Left Join
		V_VENDAS_LINHA VEM on VEA.CD_CLIENTE = VEM.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = VEM.CD_LINHA_PRODUTO
		Left Join
		V_MANUTENCAO_ULT_12 MAA on VEA.CD_CLIENTE = MAA.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = MAA.CD_LINHA_PRODUTO
		Left Join		
		V_MANUTENCAO_ULT_MES MAM on  VEA.CD_CLIENTE = MAM.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = MAM.CD_LINHA_PRODUTO
		Left Join
		V_DEPRECIACAO DEM on VEA.CD_CLIENTE = DEM.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = DEM.CD_LINHA_PRODUTO
		Left Join
		V_DEPRECIACAO_ULT_12 DEA on VEA.CD_CLIENTE = DEA.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = DEA.CD_LINHA_PRODUTO
		Left Join
		V_CUSTO_LINHA CUM on VEA.CD_CLIENTE = CUM.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = CUM.CD_LINHA_PRODUTO
		Left Join		
		V_CUSTO_LINHA_ULT_12 CUA on VEA.CD_CLIENTE = CUA.CD_CLIENTE and VEA.CD_LINHA_PRODUTO = CUA.CD_LINHA_PRODUTO
    
    	WHERE
			(CLI.CD_CLIENTE		= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
			AND (EXE.CD_EXECUTIVO		= @p_CD_EXECUTIVO		OR @p_CD_EXECUTIVO IS NULL)
			AND (VND.CD_VENDEDOR		= @p_CD_VENDEDOR		OR @p_CD_VENDEDOR IS NULL)
			AND (VND.FL_ATIVO = 'S')
			AND (LIN.CD_LINHA_PRODUTO		= @p_CD_LINHA_PRODUTO		OR @p_CD_LINHA_PRODUTO IS NULL)
			and (VEA.TOT_VENDAS > 0 OR dbo.fCountEQP(VEA.CD_CLIENTE,VEA.CD_LINHA_PRODUTO)<>0 )  

	--Select 
	--	osano.ID_OS,
	--	CLI.CD_CLIENTE,
	--	COALESCE(ISNULL(pecaos.QT_PECA,0) * ISNULL(pecaos.VL_VALOR_PECA,0),0) as VL_PECAS,
		
	--	COALESCE((tec.VL_CUSTO_HORA * (Cast((Convert(float,Substring(os.HR_FIM,0,3)) + Convert(float,Substring(os.HR_FIM,4,2))/60) 
	--	-(Convert(float,Substring(os.HR_INICIO,0,3)) + Convert(float,Substring(os.HR_INICIO,4,2))/60) 
	--		as float)) / (IIF(ISNULL(COUNT(pecaos.ID_PECA_OS),1) = 0,1,ISNULL(COUNT(pecaos.ID_PECA_OS),1) ))) + (ISNULL(pecaos.QT_PECA,0) * ISNULL(pecaos.VL_VALOR_PECA,0)),0) as VL_MAO_OBRA,
		
	--	COALESCE(ISNULL(pecaosano.QT_PECA,0) * ISNULL(pecaosano.VL_VALOR_PECA,0),0) as  TOT_PECAS,
		
	--	COALESCE((tecano.VL_CUSTO_HORA * (Cast((Convert(float,Substring(osano.HR_FIM,0,3)) + Convert(float,Substring(osano.HR_FIM,4,2))/60) 
	--	-(Convert(float,Substring(osano.HR_INICIO,0,3)) + Convert(float,Substring(osano.HR_INICIO,4,2))/60) 
	--		as float)) / (IIF(ISNULL(COUNT(pecaosano.ID_PECA_OS),1) = 0,1,ISNULL(COUNT(pecaosano.ID_PECA_OS),1) ))) + (ISNULL(pecaosano.QT_PECA,0) * ISNULL(pecaosano.VL_VALOR_PECA,0)),0) as TOT_MAO_OBRA

	--From
	--	TB_CLIENTE CLI 
		
	--	left Join
	--	tbOSPadrao os on os.CD_CLIENTE = CLI.CD_CLIENTE and os.DT_DATA_OS >= DATEADD(DAY, -30, getdate())
	--	left Join 
	--	tbPecaOs pecaos on pecaos.ID_OS = os.ID_OS
	--	left Join TB_PECA peca on peca.CD_PECA = pecaos.CD_PECA
	--	left join TB_TECNICO tec on tec.CD_TECNICO = os.CD_TECNICO

	--	left Join
	--	tbOSPadrao osano on osano.CD_CLIENTE = CLI.CD_CLIENTE and osano.DT_DATA_OS >= DATEADD(DAY, -365, getdate())
	--	left Join 
	--	tbPecaOs pecaosano on pecaosano.ID_OS = osano.ID_OS
	--	left Join TB_PECA pecaano on pecaano.CD_PECA = pecaosano.CD_PECA
	--	left join TB_TECNICO tecano on tecano.CD_TECNICO = osano.CD_TECNICO
	--	left join
	--	V_VENDAS_LINHA_ULT_12_ALL VEA on CLI.CD_CLIENTE = VEA.CD_CLIENTE
		
    
 --   	WHERE
	--		(CLI.CD_CLIENTE		= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
	--		AND os.ID_OS is not null
	--		AND ( os.ST_STATUS_OS IN (3,5) OR os.ST_STATUS_OS IS NULL)
	--		AND (pecaos.ID_OS is not null)
	--		AND osano.ID_OS is not null
	--		AND ( osano.ST_STATUS_OS IN (3,5) OR osano.ST_STATUS_OS IS NULL)
	--		AND (pecaosano.ID_OS is not null)
			
	
	--group by 
	--			 pecaos.QT_PECA,
	--			 pecaos.VL_VALOR_PECA,
	--			 pecaosano.QT_PECA,
	--			 pecaosano.VL_VALOR_PECA,
	--			 os.HR_FIM,
	--			 os.HR_INICIO,
	--			 tec.VL_CUSTO_HORA,
	--			 osano.HR_FIM,
	--			 osano.HR_INICIO,
	--			 tecano.VL_CUSTO_HORA,
	--			 CLI.CD_CLIENTE,
	--			 osano.ID_OS


	Select distinct
		os.ID_OS,
		CLI.CD_CLIENTE,
		COALESCE(ISNULL(pecaos.QT_PECA,0) * ISNULL(pecaos.VL_VALOR_PECA,0),0) as TOT_PECAS,
		
		((tec.VL_CUSTO_HORA * (Cast((Convert(float,Substring(os.HR_FIM,0,3)) + Convert(float,Substring(os.HR_FIM,4,2))/60) 
		-(Convert(float,Substring(os.HR_INICIO,0,3)) + Convert(float,Substring(os.HR_INICIO,4,2))/60) 
			as float))) ) as TOT_MAO_OBRA,
		pecaos.CD_PECA, 
		pecaos.QT_PECA,
		atv.CD_LINHA_PRODUTO,
		linha.DS_LINHA_PRODUTO
		
	From
		TB_CLIENTE CLI 
		
		left Join
		tbOSPadrao os on os.CD_CLIENTE = CLI.CD_CLIENTE and os.DT_DATA_OS >= DATEADD(Year, -1, getdate())
		left Join 
		tbPecaOs pecaos on pecaos.ID_OS = os.ID_OS
		left Join TB_PECA peca on peca.CD_PECA = pecaos.CD_PECA
		left join TB_TECNICO tec on tec.CD_TECNICO = os.CD_TECNICO
		left join TB_ATIVO_FIXO atv on atv.CD_ATIVO_FIXO = os.CD_ATIVO_FIXO
		left join TB_LINHA_PRODUTO linha on linha.CD_LINHA_PRODUTO = atv.CD_LINHA_PRODUTO
		--left Join
		--tbOSPadrao osano on osano.CD_CLIENTE = CLI.CD_CLIENTE and osano.DT_DATA_OS >= DATEADD(DAY, -365, getdate())
		--left Join 
		--tbPecaOs pecaosano on pecaosano.ID_OS = osano.ID_OS
		--left Join TB_PECA pecaano on pecaano.CD_PECA = pecaosano.CD_PECA
		--left join TB_TECNICO tecano on tecano.CD_TECNICO = osano.CD_TECNICO
		
    	WHERE
			(CLI.CD_CLIENTE		= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
			AND os.ID_OS is not null
			AND ( os.ST_STATUS_OS IN (3,5) OR os.ST_STATUS_OS IS NULL)
			--AND (pecaos.ID_OS is not null)
		
	
	group by 
				 pecaos.QT_PECA,
				 pecaos.VL_VALOR_PECA,
				 --pecaosano.QT_PECA,
				 --pecaosano.VL_VALOR_PECA,
				 os.HR_FIM,
				 os.HR_INICIO,
				 tec.VL_CUSTO_HORA,
				 --osano.HR_FIM,
				 --osano.HR_INICIO,
				 --tecano.VL_CUSTO_HORA,
				 CLI.CD_CLIENTE,
				 os.ID_OS,
				 pecaos.ID_OS,
					pecaos.CD_PECA, 
					pecaos.QT_PECA,
				 atv.CD_LINHA_PRODUTO,
				 linha.DS_LINHA_PRODUTO

					order by os.ID_OS



	Select distinct
		os.ID_OS,
		CLI.CD_CLIENTE,
		COALESCE(ISNULL(pecaos.QT_PECA,0) * ISNULL(pecaos.VL_VALOR_PECA,0),0) as VL_PECAS,
		
		((tec.VL_CUSTO_HORA * (Cast((Convert(float,Substring(os.HR_FIM,0,3)) + Convert(float,Substring(os.HR_FIM,4,2))/60) 
		-(Convert(float,Substring(os.HR_INICIO,0,3)) + Convert(float,Substring(os.HR_INICIO,4,2))/60) 
			as float)) )) as VL_MAO_OBRA,
		atv.CD_LINHA_PRODUTO,
		linha.DS_LINHA_PRODUTO
		
	From
		TB_CLIENTE CLI 
		
		left Join
		tbOSPadrao os on os.CD_CLIENTE = CLI.CD_CLIENTE and os.DT_DATA_OS >= DATEADD(MONTH, -1, getdate())
		left Join 
		tbPecaOs pecaos on pecaos.ID_OS = os.ID_OS
		left Join TB_PECA peca on peca.CD_PECA = pecaos.CD_PECA
		left join TB_TECNICO tec on tec.CD_TECNICO = os.CD_TECNICO
		left join TB_ATIVO_FIXO atv on atv.CD_ATIVO_FIXO = os.CD_ATIVO_FIXO
		left join TB_LINHA_PRODUTO linha on linha.CD_LINHA_PRODUTO = atv.CD_LINHA_PRODUTO
		--left Join
		--tbOSPadrao osano on osano.CD_CLIENTE = CLI.CD_CLIENTE and osano.DT_DATA_OS >= DATEADD(DAY, -365, getdate())
		--left Join 
		--tbPecaOs pecaosano on pecaosano.ID_OS = osano.ID_OS
		--left Join TB_PECA pecaano on pecaano.CD_PECA = pecaosano.CD_PECA
		--left join TB_TECNICO tecano on tecano.CD_TECNICO = osano.CD_TECNICO
		
		
    	WHERE
			(CLI.CD_CLIENTE		= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
			AND os.ID_OS is not null
			--AND ( os.ST_STATUS_OS IN (3,5) OR os.ST_STATUS_OS IS NULL)
			
	
	group by 
				 pecaos.QT_PECA,
				 pecaos.VL_VALOR_PECA,
				 --pecaosano.QT_PECA,
				 --pecaosano.VL_VALOR_PECA,
				 os.HR_FIM,
				 os.HR_INICIO,
				 tec.VL_CUSTO_HORA,
				 --osano.HR_FIM,
				 --osano.HR_INICIO,
				 --tecano.VL_CUSTO_HORA,
				 CLI.CD_CLIENTE,
				 os.ID_OS,
				 atv.CD_LINHA_PRODUTO,
				 linha.DS_LINHA_PRODUTO

				 order by os.ID_OS

  
  -- Data For prcRelatorioCompletosSubItensConsumidosSelect  
  
   DECLARE @p_DT_CORTE DATE = DATEADD(HH,-3,GETUTCDATE())--'2017-11-01'  
   
 CREATE TABLE #T_ItensConsumidos   
 (  
   CD_CLIENTE    numeric(6)  
  ,CD_LINHA_PRODUTO  int  
  
  ,CD_CONSUMIVEL   varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AI  
  ,DS_CONSUMIVEL   varchar(85)  
  
  ,VL_DESCONTO   numeric(15,4) NULL  
  ,VL_PRECO_PRATICADO  numeric(15,5) NULL  
  ,VL_PRECO_PRATICADO_CV numeric(15,5) NULL  
  
  ,QT_VENDAS_M  numeric(15,2) NULL  
  ,VL_VENDAS_M  numeric(15,2) NULL  
  ,QT_VENDAS_T  numeric(15,2) NULL  
  ,VL_VENDAS_T  numeric(15,2) NULL  
  ,QT_VENDAS_A  numeric(15,2) NULL  
  ,VL_VENDAS_A  numeric(15,2) NULL  
 )  
  
  --Query principal  
  INSERT INTO #T_ItensConsumidos ( CD_CLIENTE, CD_LINHA_PRODUTO, CD_CONSUMIVEL, DS_CONSUMIVEL )  
  SELECT      VEN.CD_CLIENTE,       
     CON.CD_LINHA_PRODUTO,        
     CON.CD_CONSUMIVEL,        
     CON.DS_CONSUMIVEL--RTRIM(REPLACE(REPLACE(CON.DS_CONSUMIVEL,'-',''), CON.CD_CONSUMIVEL, ''))  
  FROM      TB_VENDAS_CLIENTE VEN   
  INNER JOIN      TB_CONSUMIVEL CON   
   ON CON.CD_CONSUMIVEL = VEN.CD_CONSUMIVEL    
  INNER JOIN      TB_CLIENTE CLI   
   ON CLI.CD_CLIENTE = VEN.CD_CLIENTE    
  
  INNER JOIN #TB_FILTRO_VENDAS FV ON FV.CD_CLIENTE = CLI.CD_CLIENTE AND FV.CD_LINHA_PRODUTO = CON.CD_LINHA_PRODUTO  
  
  INNER JOIN      TB_VENDEDOR VND   
   ON VND.CD_VENDEDOR = CLI.CD_VENDEDOR    
  
  WHERE CD_MES >= convert(numeric(6),LEFT(CONVERT(VARCHAR(8),DATEADD(m,-12,@p_DT_CORTE),112),6))    
  AND (CLI.CD_CLIENTE   = @p_CD_CLIENTE   OR @p_CD_CLIENTE IS NULL)  
  AND (CLI.CD_EXECUTIVO  = @p_CD_EXECUTIVO  OR @p_CD_EXECUTIVO IS NULL)  
  AND (CLI.CD_VENDEDOR  = @p_CD_VENDEDOR  OR @p_CD_VENDEDOR IS NULL)
  AND (VND.FL_ATIVO = 'S')
  AND (CON.CD_LINHA_PRODUTO = @p_CD_LINHA_PRODUTO OR @p_CD_LINHA_PRODUTO IS NULL)  
   
  GROUP BY VEN.CD_CLIENTE,     CON.CD_LINHA_PRODUTO,      CON.CD_CONSUMIVEL,      CON.DS_CONSUMIVEL   
  
  CREATE TABLE #T_TMP_VL_VENDAS  
  (  
    CD_CLIENTE    numeric(6)  
   ,CD_CONSUMIVEL   varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AI  
  
    ,VL_DESCONTO   numeric(15,4) NULL  
   ,VL_PRECO_PRATICADO  numeric(15,5) NULL  
   ,VL_PRECO_PRATICADO_CV numeric(15,5) NULL  
  
   ,QT_VENDA_M  numeric(15,2) NULL  
   ,VL_VENDA_M  numeric(15,2) NULL  
   ,QT_VENDA_T  numeric(15,2) NULL  
   ,VL_VENDA_T  numeric(15,2) NULL  
   ,QT_VENDA_A  numeric(15,2) NULL  
   ,VL_VENDA_A  numeric(15,2) NULL  
  )  
  
  UPDATE #T_ItensConsumidos SET  
    VL_DESCONTO = dbo.fDesconto(T.CD_CLIENTE, T.CD_CONSUMIVEL)  * 100  
   ,VL_PRECO_PRATICADO = PP.PRECO_PRATICADO  
   ,VL_PRECO_PRATICADO_CV = PP.PRECO_PRATICADO_CV  
  FROM #T_ItensConsumidos T  
  INNER JOIN V_PRECO_PRATICADO PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
  
  INSERT #T_TMP_VL_VENDAS (CD_CLIENTE, CD_CONSUMIVEL, QT_VENDA_M, VL_VENDA_M)  
  SELECT T.CD_CLIENTE, T.CD_CONSUMIVEL  
    ,SUM(ISNULL(QT_VENDAS,0))  
    ,SUM(ISNULL(QT_VENDAS,0) * ISNULL(PR_PRATICADO,0))   
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO = 3  
  WHERE CD_MES = CONVERT(NUMERIC(6), LEFT(CONVERT(VARCHAR(8), DATEADD(m, -1, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  UNION ALL  
  SELECT  T.CD_CLIENTE, T.CD_CONSUMIVEL  
    ,SUM(ISNULL(QT_VENDAS_CV,0))  
    ,SUM(ISNULL(QT_VENDAS ,0) * ISNULL(PR_PRATICADO,0)) --??VB6 COM QT_VENDAS  
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO <> 3  
  WHERE CD_MES = CONVERT(NUMERIC(6), LEFT(CONVERT(VARCHAR(8), DATEADD(m, -1, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  
  
  INSERT #T_TMP_VL_VENDAS ( CD_CLIENTE, CD_CONSUMIVEL, QT_VENDA_T, VL_VENDA_T)  
  SELECT  T.CD_CLIENTE, T.CD_CONSUMIVEL  
    ,SUM(ISNULL(QT_VENDAS,0))  
    ,SUM(ISNULL(QT_VENDAS,0) * ISNULL(PR_PRATICADO,0))   
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO = 3  
  WHERE CD_MES >= convert(Numeric(6), Left(convert(VarChar(8), DateAdd(m, -3, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  UNION ALL  
  SELECT  T.CD_CLIENTE, T.CD_CONSUMIVEL  
    ,SUM(ISNULL(QT_VENDAS_CV,0))  
    ,SUM(ISNULL(QT_VENDAS,0) * ISNULL(PR_PRATICADO,0)) --??VB6 COM QT_VENDAS  
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO <> 3  
  WHERE CD_MES >= CONVERT(NUMERIC(6), LEFT(CONVERT(VARCHAR(8), DATEADD(m, -3, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  
  INSERT #T_TMP_VL_VENDAS ( CD_CLIENTE, CD_CONSUMIVEL, QT_VENDA_A, VL_VENDA_A )  
  SELECT  T.CD_CLIENTE, T.CD_CONSUMIVEL  
    ,SUM(ISNULL(QT_VENDAS,0))  
    ,SUM(ISNULL(QT_VENDAS,0) * ISNULL(PR_PRATICADO,0))   
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO = 3  
  WHERE CD_MES  >= convert(Numeric(6), Left(convert(VarChar(8), DateAdd(m, -12, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  UNION ALL  
  SELECT T.CD_CLIENTE, T.CD_CONSUMIVEL   
    ,SUM(ISNULL(QT_VENDAS_CV,0))  
    ,SUM(ISNULL(QT_VENDAS,0) * ISNULL(PR_PRATICADO,0)) --??VB6 COM QT_VENDAS  
  FROM #T_ItensConsumidos T  
  INNER JOIN TB_VENDAS_CLIENTE PP  
   ON PP.CD_CLIENTE = T.CD_CLIENTE   
   AND PP.CD_CONSUMIVEL = T.CD_CONSUMIVEL  
   AND T.CD_LINHA_PRODUTO <> 3  
  WHERE CD_MES  >= convert(Numeric(6), Left(convert(VarChar(8), DateAdd(m, -12, @p_DT_CORTE), 112), 6))   
  GROUP BY T.CD_CLIENTE, T.CD_CONSUMIVEL  
  
  UPDATE #T_ItensConsumidos SET  
    QT_VENDAS_M = T.QT_VENDA_M  
   ,VL_VENDAS_M = T.VL_VENDA_M  
   ,QT_VENDAS_T = T.QT_VENDA_T  
   ,VL_VENDAS_T = T.VL_VENDA_T  
   ,QT_VENDAS_A = T.QT_VENDA_A  
   ,VL_VENDAS_A = T.VL_VENDA_A  
  FROM #T_TMP_VL_VENDAS T  
  WHERE T.CD_CLIENTE = #T_ItensConsumidos.CD_CLIENTE   
  AND T.CD_CONSUMIVEL  COLLATE SQL_Latin1_General_CP1_CI_AI= #T_ItensConsumidos.CD_CONSUMIVEL  COLLATE SQL_Latin1_General_CP1_CI_AI  
  
 SELECT * FROM #T_ItensConsumidos   
  
 if (object_id('tempdb..#T_ItensConsumidos') is not null)  
  DROP TABLE #T_ItensConsumidos   
  
 if (object_id('tempdb..#T_TMP_VL_VENDAS') is not null)  
  DROP TABLE #T_TMP_VL_VENDAS  
  
  
  
  
  
  
  
  
    
 END TRY  
  
 BEGIN CATCH  
  
  SELECT @cdsErrorMessage = ERROR_MESSAGE(),  
    @nidErrorSeverity = ERROR_SEVERITY(),  
    @nidErrorState  = ERROR_STATE();  
  
  -- Use RAISERROR inside the CATCH block to return error  
  -- information about the original error that caused  
  -- execution to jump to the CATCH block.  
  RAISERROR (@cdsErrorMessage, -- Message text.  
       @nidErrorSeverity, -- Severity.  
       @nidErrorState -- State.  
       )  
  
 END CATCH  
  
END  
