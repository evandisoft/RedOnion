if exists("b:current_syntax")
  finish
endif
let b:current_syntax = "redonion"

hi def link rosStatement		Statement
hi def link rosLabel			Label
hi def link rosConditional		Conditional
hi def link rosRepeat			Repeat
hi def link rosOperator			Operator
hi def link rosStructure		Structure
hi def link rosStorageClass		StorageClass
hi def link rosType				Type
hi def link rosConstant			Constant
hi def link rosComment			Comment
hi def link rosDocComment		Preproc
hi def link rosDocCommentBody	Comment
hi def link rosPreproc			Preproc
hi def link rosPreCmd			Statement
hi def link rosSpecial			SpecialChar
hi def link rosError			Error
hi def link rosNumber			Number
hi def link rosString			String
hi def link rosCharacter		Character
hi def link rosDebug			Debug
hi def link rosAccess			rosStatement
hi def link rosPreString		rosString
hi def link rosFormat			rosSpecial
hi def link rosPreSpec			rosNumber
hi def link rosPreID			rosNumber
hi def link rosFloat			rosNumber
hi def link rosOctal			rosNumber
hi def link rosOctalError		rosError
hi def link rosCommentLine		rosComment
hi def link rosDocCommentLine	rosDocComment
hi def link rosDocCommentLBdy	rosDocCommentBody
hi def link rosParenError		rosError

syn case match

syn keyword rosStatement		return break continue goto
syn keyword rosStatement		yield function def var new using import
syn keyword rosStatement		raise throw try catch finally
syn keyword rosStatement		as in is out ref params lock
syn keyword rosStatement		get set add remove value
syn keyword rosConditional		if unless then else switch
syn keyword rosLabel			case default
syn match	rosLabel			display "^\s*\I\i*:$"
syn match	rosLabel			display ";\s*\I\i*:$"
syn match	rosLabel			display "^\s*\I\i*:[^:]"me=e-1
syn match	rosLabel			display ";\s*\I\i*:[^:]"me=e-1
syn match	rosRepeat			display "\<\(while\|until\|do\|for\|foreach\)\>"
syn keyword rosOperator			and or not
syn keyword rosConstant			true false null this base super

syn match	rosStructure		display "\<\(class\|namespace\|interface\|struct\|union\|enum\)\>"
syn match	rosAccess			display "\<\(public\|protected\|internal\|private\|friend\)\>"
syn match	rosStorageClass		display "\<\(static\|const\|virtual\|delegate\|event\|partial\|abstract\|where\)\>"
syn match	rosType				display	"\<\(void\|bool\|char\|int\|long\|short\|small\|uint\|word\|byte\|float\|double\|decimal\|object\|sbyte\|string\|ulong\|ushort\)\>"

"number
syn case ignore
syn match	rosNumbers			display transparent "\<\d\|\.\d" contains=rosNumber,rosFloat,rosOctalError,rosOctal
syn match	rosNumber			display contained "\d\+\(u\=l\{0,2}\|ll\=u\)\>"
syn match	rosNumber			display contained "0x\x\+\(u\=l\{0,2}\|ll\=u\)\>"
syn match	rosOctal			display contained "0\o\+\(u\=l\{0,2}\|ll\=u\)\>"
syn match	rosFloat			display contained "\d\+f"
syn match	rosFloat			display contained "\d\+\.\d*\(e[-+]\=\d\+\)\=[fl]\="
syn match	rosFloat			display contained "\.\d\+\(e[-+]\=\d\+\)\=[fl]\=\>"
syn match	rosFloat			display contained "\d\+e[-+]\=\d\+[fl]\=\>"
syn match	rosFloat			display contained "0x\x*\.\x\+p[-+]\=\d\+[fl]\=\>"
syn match	rosFloat			display contained "0x\x\+\.\=p[-+]\=\d\+[fl]\=\>"
syn match	rosOctalError		display contained "0\o*[89]\d*"
syn cluster rosContained		add=rosNumber,rosOctal,rosFloat,rosOctalError
syn case match



"string
syn match	rosSpecial			display contained @\\\([abefnrtv'"\\?]\|x\x\+\|\o\{1,3}\|u\x\{4}\|U\x\{8}\|$\)@
syn match	rosFormat			display contained "%%"
syn match	rosFormat			display contained "%\(\d\+\$\)\=[-+' #0*]*\(\d*\|\*\|\*\d\+\$\)\(\.\(\d*\|\*\|\*\d\+\$\)\)\=\([hlLjzt]\|ll\|hh\)\=\([aAbdiuoxXDOUfFeEgGcCsSpn]\|\[\^\=.[^]]*\]\)"

syn region	rosString			start=+L\="+ skip=+\\\\\|\\"+ end=+"+ contains=rosSpecial,rosFormat,rosNumbers
syn region	rosPreString		contained start=+L\="+ skip=+\\\\\|\\"+ end=+"+ excludenl end='$' contains=rosSpecial,rosFormat,rosNumbers
syn match	rosCharacter		"L\='[^\\]'"
syn match	rosCharacter		@L\='\\\([abefnrtv'"\\?]\|x\x\+\|\o\{1,3}\|u\x\{4}\|U\x\{8}\|$\)'@
syn match	rosCharacter		"L'[^']*'" contains=rosSpecial
syn cluster rosContained		add=rosSpecial,rosFormat,rosPreString



"comment
syn region	rosCommentLine		start="//\([^/]\|$\)" skip="\\$" end="$" keepend
syn region	rosComment			start="/\*\([^*!@]\|$\)" end="\*/" fold extend
syn region	rosDocCommentLine	start="//[/!@]" skip="\\$" end="$" keepend contains=rosPreString,rosPreNumbers,rosPreID,rosDocCommentLBdy
syn region	rosDocCommentLBdy	contained display start="//[/!@]<\="ms=e+1 skip="\\$" excludenl end="$"
syn region	rosDocComment		start="/\*[*!@]" end="\*/" fold extend keepend contains=rosPreString,rosPreNumbers,rosPreID,rosDocCommentBody
syn region	rosDocCommentBody	contained display start="/\*[*!@]"ms=e+1 end="\*/"me=s-1
syn cluster	rosContained		add=rosDocCommentBody



"parenthesis
syn region	rosParen			transparent start='(' end=')' contains=ALLBUT,@rosContained,rosParenError
syn match	rosParenError		display "[)]"



" sync
syn sync minlines=50

" vim: ts=4
