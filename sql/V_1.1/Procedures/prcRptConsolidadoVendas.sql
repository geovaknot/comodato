--Querie Relatorio Vendas consolidado 
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--use COMODATOhom
--execute prcRptConsolidadoVendas '3280'

-- ================================================
-- Author:	Roberto Góes 
-- Alter date: 09/02/2022
-- Description:	Relatório Consolidado de Vendas
-- ================================================
CREATE PROCEDURE [dbo].[prcRptConsolidadoVendas]
	@p_CD_CLIENTES		VARCHAR(MAX)	= NULL, --'3280,',
	@p_CD_GRUPOS		VARCHAR(MAX)	= NULL

AS
 
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@CD_TECNICO			VARCHAR(06)

			--@p_CD_CLIENTES		VARCHAR(MAX)	= '3222,',
			--@p_CD_GRUPOS		VARCHAR(MAX)	= NULL

	SET NOCOUNT ON;
	SET FMTONLY OFF;

	BEGIN TRY
	
		Declare	@p_DT_FINAL DATETIME = CONVERT(date, DATEADD(HH,-3,GETUTCDATE())) --Hoje 
		Declare	@p_DT_INICIAL DATETIME = DATEADD(MONTH, -1, @p_DT_FINAL) -- 12 Meses a menos 

		If Exists(Select * from tempdb..SysObjects Where Name Like '%T_RelConsolVenda_ContaPecaOS' ) drop table #T_RelConsolVenda_ContaPecaOS;
		If Exists(Select * from tempdb..SysObjects Where Name Like '%TT_VISITA' ) drop table #TT_VISITA;
		If Exists(Select * from tempdb..SysObjects Where Name like '%TT_QTD_TECNICOS' ) drop table #TT_QTD_TECNICOS;
		If Exists(Select * from tempdb..SysObjects Where Name like '%TT_TECNICOS_ORDEM' ) drop table #TT_TECNICOS_ORDEM;

		--Geral de Manutenção
		SELECT 
			osPad.ID_OS, IIF(ISNULL(COUNT(tabPecaOS.ID_PECA_OS),1) = 0,1,ISNULL(COUNT(tabPecaOS.ID_PECA_OS),1) ) AS ContaPeca 
		INTO #T_RelConsolVenda_ContaPecaOS
		FROM dbo.TB_TECNICO
		LEFT JOIN tbOSPadrao osPad ON dbo.TB_TECNICO.CD_TECNICO = osPad.CD_TECNICO AND osPad.ST_STATUS_OS in (3,5)
		LEFT JOIN dbo.TB_CLIENTE ON osPad.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
		LEFT JOIN dbo.tbPecaOS tabPecaOS ON osPad.ID_OS = tabPecaOS.ID_OS
		LEFT JOIN dbo.TB_PECA tabPeca ON tabPecaOS.CD_PECA = tabPeca.CD_PECA
		LEFT JOIN dbo.TB_ATIVO_FIXO ON dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO = osPad.CD_ATIVO_FIXO
		LEFT JOIN dbo.TB_MODELO ON dbo.TB_MODELO.CD_MODELO = dbo.TB_ATIVO_FIXO.CD_MODELO
		LEFT JOIN dbo.TB_GRUPO ON dbo.TB_CLIENTE.CD_GRUPO = dbo.TB_GRUPO.CD_GRUPO
		WHERE
			TB_TECNICO.FL_ATIVO = 'S'
			AND ( osPad.ST_STATUS_OS IN (3,5) OR osPad.ST_STATUS_OS IS NULL)	
			AND ( dbo.TB_CLIENTE.CD_CLIENTE	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
			AND ( dbo.TB_GRUPO.CD_GRUPO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_GRUPOS, ','))			OR @p_CD_GRUPOS			IS NULL )
			AND (osPad.DT_DATA_OS 	>= @p_DT_INICIAL	OR @p_DT_INICIAL		IS NULL)  
			AND (osPad.DT_DATA_OS 	<= @p_DT_FINAL	OR @p_DT_FINAL			IS NULL )
		GROUP BY
			osPad.ID_OS

		--Kat
        SELECT distinct
		vs.Id_OS,
		vs.CD_TECNICO,
		vs.CD_CLIENTE 
		, Cast((Convert(float,Substring(vs.HR_FIM,0,3)) + Convert(float,Substring(vs.HR_FIM,4,2))/60) - (Convert(float,Substring(vs.HR_INICIO,0,3)) + Convert(float,Substring(vs.HR_INICIO,4,2))/60)  as float) as Horas
		INTO #TT_VISITA
		FROM tbOsPadrao vs
		LEFT JOIN tbLogStatusOsPadrao lsv 
		ON lsv.ID_OS = vs.ID_OS and lsv.ST_STATUS_OS IN (3)
		WHERE vs.ST_STATUS_OS IN (3) 
				AND vs.dtmDataHoraAtualizacao BETWEEN @p_DT_INICIAL AND @p_DT_FINAL OR (@p_DT_FINAL IS NULL AND @p_DT_INICIAL IS NULL)
				AND ( vs.CD_CLIENTE IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ',')) OR @p_CD_CLIENTES		IS NULL )
		GROUP BY vs.ID_OS, vs.CD_TECNICO, vs.CD_Cliente, vs.HR_FIM, vs.HR_INICIO

		select CD_CLIENTE, count(CD_ORDEM) as QtdTecnicos 
		INTO #TT_QTD_TECNICOS
		from TB_TECNICO_CLIENTE
		GROUP BY CD_CLIENTE

		select CD_CLIENTE, CD_TECNICO, CD_ORDEM
		INTO #TT_TECNICOS_ORDEM
		from TB_TECNICO_CLIENTE
		
		

		SELECT Distinct
			cli.CD_CLIENTE
			,cli.NM_CLIENTE
			--Consumo Cliente 
			,CASE WHEN LIN.CD_LINHA_PRODUTO =3 THEN VEA.QT_VENDAS ELSE VEA.QT_VENDAS_CV END AQT_VENDAS
            , COALESCE(VEA.TOT_VENDAS,0) ATOT_VENDAS
			,dbo.fCountEQP(cli.CD_CLIENTE,VEA.CD_LINHA_PRODUTO) QT_EQP
			--Geral de Manutenção
			,ISNULL(SUM(peca.QT_PECA), 0) AS QT_PECAS
			,ISNULL(SUM((peca.QT_PECA * PC.VL_PECA)), 0) AS VL_PECAS 
			,ISNULL(SUM(Cast(Convert(float,Replace(osPad.HR_FIM,':','.')) - Convert(float,Replace(osPad.HR_INICIO,':','.')) as float)/ISNULL(ContaPecaOS.ContaPeca,1)), 0) AS QT_HORAS
			,ISNULL(SUM((tec.VL_CUSTO_HORA * Cast(Convert(float,Replace(osPad.HR_FIM,':','.')) - Convert(float,Replace(osPad.HR_INICIO,':','.')) as float)/ISNULL(ContaPecaOS.ContaPeca,1))), 0) AS VL_HORAS
			-- Kat
			, ISNULL(SUM(vs.Horas)/3,0) AS KatRealizado
			, CASE WHEN ord.CD_ORDEM <> 1 THEN 0 ELSE ISNULL((cli.QT_PERIODO/12.0), 0) END as KatMesPorTecnico
			-- GM(%)
			,Round(((( 1 - CUS.CUSTO - VEA.DEPCOM - (CASE WHEN (VEL.TOT_VENDAS <> 0) THEN COALESCE(DEP.TOT_DEPRECIACAO,0) / VEL.TOT_VENDAS Else 0  END  )
						- ( CASE WHEN (VEL.TOT_VENDAS <> 0) THEN (COALESCE(MAN.TOT_PECAS,0) + COALESCE(MAN.TOT_MAO_OBRA,0)) / VEL.TOT_VENDAS  Else 0  END) ) * 100 )
						- (VEA.LESAFO * 100)) / 100 * VEA.TOT_VENDAS,0) AS OIR
			-- OI(%)
			,Round(( 1 - CUS.CUSTO - VEA.DEPCOM - (CASE WHEN (VEL.TOT_VENDAS <> 0) THEN COALESCE(DEP.TOT_DEPRECIACAO,0) / VEL.TOT_VENDAS Else 0  END  )
				      - ( CASE WHEN (VEL.TOT_VENDAS <> 0) THEN (COALESCE(MAN.TOT_PECAS,0) + COALESCE(MAN.TOT_MAO_OBRA,0)) / VEL.TOT_VENDAS  Else 0  END) ) * 100,2) / 100 AS fGM
 
   

		FROM dbo.TB_TECNICO tec
		LEFT JOIN tbOSPadrao osPad ON tec.CD_TECNICO = osPad.CD_TECNICO
		LEFT JOIN tbLogStatusOSPadrao logOs on osPad.ID_OS = logOs.ID_OS AND ( logOs.ST_STATUS_OS IN (3,5) OR logOs.ST_STATUS_OS IS NULL)
		LEFT JOIN dbo.TB_CLIENTE cli ON osPad.CD_CLIENTE = cli.CD_CLIENTE

		left join V_VENDAS_CONSUMIVEL_ULT_12 as VEA on VEA.CD_CLIENTE = cli.CD_CLIENTE
		Left Join TB_CONSUMIVEL CON on CON.CD_CONSUMIVEL = VEA.CD_CONSUMIVEL
		left join TB_LINHA_PRODUTO LIN on LIN.CD_LINHA_PRODUTO = VEA.CD_LINHA_PRODUTO
		Left Join V_CUSTO_CONSUMIVEL_ULT_12 CUS on CUS.CD_CLIENTE = VEA.CD_CLIENTE and CUS.CD_CONSUMIVEL = CON.CD_CONSUMIVEL
		left join V_VENDAS_LINHA_ULT_12_ALL VEL on VEL.CD_CLIENTE = VEA.CD_CLIENTE AND VEL.CD_LINHA_PRODUTO = VEA.CD_LINHA_PRODUTO
		Left Join V_DEPRECIACAO_ULT_12 DEP on DEP.CD_CLIENTE = VEA.CD_CLIENTE AND DEP.CD_LINHA_PRODUTO = VEA.CD_LINHA_PRODUTO
		Left Join V_MANUTENCAO_ULT_12 MAN on MAN.CD_CLIENTE = VEA.CD_CLIENTE AND MAN.CD_LINHA_PRODUTO = VEA.CD_LINHA_PRODUTO

		LEFT JOIN dbo.tbPecaOS peca ON osPad.ID_OS = peca.ID_OS
		LEFT JOIN dbo.TB_PECA PC ON peca.CD_PECA = PC.CD_PECA
		LEFT JOIN dbo.TB_ATIVO_FIXO ATV ON ATV.CD_ATIVO_FIXO = osPad.CD_ATIVO_FIXO
		LEFT JOIN dbo.TB_MODELO mode ON mode.CD_MODELO = ATV.CD_MODELO
		LEFT JOIN dbo.TB_GRUPO GP ON cli.CD_GRUPO = GP.CD_GRUPO
		LEFT JOIN #T_RelConsolVenda_ContaPecaOS ContaPecaOS ON ContaPecaOS.ID_OS = osPad.ID_OS

	    --Kat
		LEFT JOIN TB_TECNICO_CLIENTE  ON TB_TECNICO_CLIENTE.CD_CLIENTE = cli.CD_CLIENTE and TB_TECNICO_CLIENTE.CD_TECNICO = tec.CD_TECNICO
		LEFT JOIN #TT_VISITA vs ON vs.CD_TECNICO = TB_TECNICO_CLIENTE.CD_TECNICO AND vs.CD_CLIENTE = TB_TECNICO_CLIENTE.CD_CLIENTE
		INNER JOIN #TT_QTD_TECNICOS qtd on qtd.CD_CLIENTE = TB_TECNICO_CLIENTE.CD_CLIENTE
		INNER JOIN #TT_TECNICOS_ORDEM ord on ord.CD_TECNICO = tec.CD_TECNICO AND ord.CD_CLIENTE = cli.CD_CLIENTE

		WHERE
			tec.FL_ATIVO = 'S'
			AND ( osPad.ST_STATUS_OS IN (3,5) OR osPad.ST_STATUS_OS IS NULL)
			AND (osPad.ID_OS IS NOT NULL)
			AND ( cli.CD_CLIENTE	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ',')) OR @p_CD_CLIENTES	IS NULL )
			AND ( GP.CD_GRUPO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_GRUPOS, ','))	OR @p_CD_GRUPOS	IS NULL )
			AND (osPad.DT_DATA_OS 	>= @p_DT_INICIAL	OR @p_DT_INICIAL		IS NULL)  
			AND (osPad.DT_DATA_OS 	<= @p_DT_FINAL	OR @p_DT_FINAL			IS NULL )
			--Kat
			AND ( TB_TECNICO_CLIENTE.CD_CLIENTE IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ',')) OR @p_CD_CLIENTES	IS NULL )


		GROUP BY
			 osPad.ID_OS
			,osPad.CD_CLIENTE
			,cli.CD_CLIENTE
			,cli.NM_CLIENTE
			,tec.CD_TECNICO
			,tec.NM_TECNICO
			,osPad.CD_ATIVO_FIXO
			,mode.CD_MODELO
			,mode.DS_MODELO
			,GP.CD_GRUPO
			,GP.DS_GRUPO
			, osPad.DT_DATA_OS
			,ContaPecaOS.ContaPeca
			, osPad.ID_OS
			,CUS.CUSTO
			 ,VEA.DEPCOM 
			 ,VEL.TOT_VENDAS
			 ,DEP.TOT_DEPRECIACAO
			 ,MAN.TOT_PECAS
			 ,MAN.TOT_MAO_OBRA
			 ,VEA.LESAFO
			 ,VEA.TOT_VENDAS
			 ,ord.CD_ORDEM
			 ,cli.QT_PERIODO
			 ,VEA.CD_LINHA_PRODUTO
			 ,LIN.CD_LINHA_PRODUTO
			 ,VEA.QT_VENDAS 
			 ,VEA.QT_VENDAS_CV
		 
		drop table #T_RelConsolVenda_ContaPecaOS;
		drop table #TT_VISITA;
		drop table #TT_QTD_TECNICOS;
		drop table #TT_TECNICOS_ORDEM;

	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH




END 




