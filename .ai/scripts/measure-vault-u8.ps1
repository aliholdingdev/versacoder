<#
.SYNOPSIS
    measure-vault.ps1 UTF-8 sarmalayicisi (PS 5.1 ANSI okuma sorunu icin).
.DESCRIPTION
    Ana script UTF-8 (BOMsuz) yazildigi icin PS 5.1 ANSI okur ve parse hatasi verir.
    Bu sarmalayici dosyayi UTF-8 olarak okuyup calistirir; cikti aynidir.
.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .ai\scripts\measure-vault-u8.ps1
.NOTES
    authority: SSOT - 2026-09-25
#>
[CmdletBinding()]
param(
    [string]$Root = 'C:\versacoder\.ai',
    [string]$Json = ''
)

$ErrorActionPreference = 'Stop'
$here  = Split-Path -Parent $MyInvocation.MyCommand.Path
$src   = Join-Path $here 'measure-vault.ps1'
$text  = [IO.File]::ReadAllText($src, [Text.Encoding]::UTF8)
$sb    = [scriptblock]::Create($text)
& $sb -Root $Root -Json $Json
