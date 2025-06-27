$startDate = Get-Date
$chromePath = "C:\Program Files\Google\Chrome\Application\chrome.exe"
$baseUrl = "https://www.str8ts.com/feed/derwesten/ASStr8tsv2.asp"
$outputBasePath = "D:\Jens\Repositories\Str8tsSolver\Str8tsSolverTest\DerWesten"
$targetBasePath = ".\Samples_DerWesten"

function Get-Str8tsBoard {
    param (
        [Parameter(Mandatory = $true)]
        [string]$FilePath
    )

    # Read the HTML content
    $htmlContent = Get-Content -Path $FilePath -Raw

    # Create HTML object
    $html = New-Object -Com "HTMLFile"
    $html.IHTMLDocument2_write($htmlContent)
 
    # Find the board table
    $boardTable = $html.getElementById("boardtable")
    if ($null -eq $boardTable) {
        Write-Error "Board table not found in HTML"
        return $null
    }
 
    # Initialize 9x9 array for the board
    $board = New-Object 'char[,]' 9,9
 
    # Get all cells from the table
    $cells = $boardTable.getElementsByTagName("td")
    $row = 0
    $col = 0

    foreach ($cell in $cells) {
        if ($cell.id -match "C(\d)(\d)") {
            $row = [int]$Matches[1]
            $col = [int]$Matches[2]
            $value = $cell.innerText.Trim()
           
            # Convert empty cells to spaces
            if ([string]::IsNullOrEmpty($value)) {
                $value = ' '
            }
           
            # Handle black cells with numbers (convert to letters A-I)
            if ($cell.className -match "CellBlack") {
                if ($value -eq " ") {
                    $value = '#'
                }
                else {
                    $value = [char]([int][char]'A' + [int]$value - 1)
                }
            }
           
            $board[$row,$col] = $value
        }
    }
 
    # Release COM object
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($html) | Out-Null
 
    return $board
}
 
# Example usage:
# $board = Get-Str8tsBoard -FilePath "$outputBasePath\2024-03-10.txt"
# Display the board
function Show-Str8tsBoard {
    param (
        [Parameter(Mandatory = $true)]
        [char[]]$board
    )
   
    for ($i = 0; $i -lt 9; $i++) {
        $row = ""
        for ($j = 0; $j -lt 9; $j++) {
            $x = $board[$i*9+$j]
            $row += $x
        }
        Write-Output $row
    }
}
 
for ($d = 0; $d -lt 30; $d++) {
    $currentDate = $startDate.AddDays(-$d)
    $dateStr = $currentDate.ToString("yyyy-MM-dd")
    Write-Output $dateStr
    & $chromePath --headless --dump-dom "$baseUrl`?d=$d" | echo > "$outputBasePath\$dateStr.txt"
 
    $board = Get-Str8tsBoard -FilePath "$outputBasePath\$dateStr.txt"

    Show-Str8tsBoard -board $board

    # Save board to file
    $boardText = @()
    for ($i = 0; $i -lt 9; $i++) {
        $row = ""
        for ($j = 0; $j -lt 9; $j++) {
            $c = $board[$i*9+$j]
            if ($c -eq ' ') {
              $c = '.'
            }
            $row += $c
        }
        $boardText += $row
    }
    $boardText | Out-File -FilePath "$targetBasePath\board_$dateStr.txt"
}
 
Write-Output "Done"
