# Wahren.Compiler

コマンドラインツールとしてのフロント
DebugPaperやScript Datなどをメモリに展開して解析する

# Wahren.FileLoader

UnicodeHandlerについて説明する(Cp932HandlerはこれのShift-JIS版)
static ValueTask<DualList<char>> LoadAsync(SafeFileHandle, CancellationToken)がメインの関数である
戻り値のDualList<char>はファイルの行のリストである
各行に改行文字('\r', '\n')は含まれていない
元ファイルがVahren流の暗号化をされている場合自動で復号する機能をもつ

LexerやParserは主にこのDualList<char>を対象に動作する

# Wahren.DebugPaper

1番最初に読み込まないと後全てが何も進まない存在であるdebug_paper.txtを解析している

# Wahren.AbstractSytanxTree.TextTemplateHelper

Vahrenの抽象構文木(AST)を手作業で構築するのは非現実的であるためT4 Templateの技術を利用して諸々自動生成することにしている
このプロジェクトに記された注釈を元にしてWahren.AbstractSytanxTreeにおいてValidationコードが自動生成されるのだ

## CallableInfo.cs

ここでは主に関数ごとの名前とそれに対応した引数の個数と型の注釈を記述している

## ElementInfo.cs

ここでは主にVahrenでいう「構造体」の「要素」それぞれについて型の注釈を行っている
またそれぞれ「構造体」の種類ごとにどのような「要素」が含まれるかの定義もしている

# Wahren.AbstractSyntaxTree

実際に構文解析を行い、粗漏がないかバリデーションするためのAPIを提供しているプロジェクトがこれ
構文解析自体は2段階に分けて行なうことを想定している

**ファイルごとの構文解析:** 基本的に「構造体」は各ファイル内で完結している存在である
大体の記述ミスはファイル単体をチェックすれば指摘可能
ここの部分はマルチスレッドに実行されるべきである

* Parserフォルダ以下が構文木構築を担当している
* PerResultValidatorフォルダ以下が各ファイル毎のバリデーションを担当する
* Nodeフォルダは各「構造体」の構文木と各「項目」のデータホルダを定義している
* 「scenario/event/story構造体」では「項目」設定以外に「関数」呼び出しなどを記述することが可能である
  * Statementフォルダは特にこの「関数」たちについて特別な構文木を構築している
  * Statement/Expressionフォルダ以下では特に「if/rif/whike文」の「条件式」について専門に取り扱っている

**プロジェクト丸ごとの構文解析**　全てのファイルに対して構文木を構築した後、プロジェクト横断的に参照のバリデーションを行なう(Projectフォルダ以下のcsファイルが担当)

### VariantPairについて

「要素名@ヴァリアント名」という表記法がたまに見られる
「ヴァリアント名」は大抵シナリオを指しているが、「dungeon構造体」などでは「階数」を指す数値であるなどたまに例外がある
このためScenarioVariantPairではなくVariantPairという名前になっている