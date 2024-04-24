USE [COMODATODEV]
GO
/****** Object:  StoredProcedure [dbo].[prcClienteDetalheManutencaoSelect]    Script Date: 24/03/2023 14:24:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcClienteDetalheManutencaoSelect]
	@p_CD_CLIENTE		NUMERIC(6,0)
AS
BEGIN


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

END