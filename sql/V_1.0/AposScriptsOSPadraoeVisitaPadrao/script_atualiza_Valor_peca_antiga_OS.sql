--select * from tbPecaOS where vl_valor_peca is null
update pecaOS
	set pecaOS.VL_VALOR_PECA = peca.VL_PECA
from tbPecaOS as pecaOS
inner join TB_PECA peca on
	peca.CD_PECA = pecaOS.CD_PECA
where pecaOS.VL_VALOR_PECA is null


