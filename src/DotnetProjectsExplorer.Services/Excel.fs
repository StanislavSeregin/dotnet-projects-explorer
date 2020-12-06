module Excel

open NPOI.HSSF.UserModel
open System.IO

let excelStream<'T> (titleWithFuncSeq: (string * ('T -> string)) seq)
                    (data: 'T seq)
                    (sheetName: string) =

    let book = HSSFWorkbook ()
    let sheet = book.CreateSheet sheetName
    let headerRow = sheet.CreateRow 0
    let indexWithTitleAndFunc =
        titleWithFuncSeq
        |> Seq.mapi (fun i (title, func) -> (i, title, func))
        |> Array.ofSeq

    indexWithTitleAndFunc
    |> Array.iter (fun (index, title, _) ->
        headerRow.CreateCell index
        |> fun cell -> cell.SetCellValue title)

    data
    |> Seq.iteri (fun rowIndex item ->
        sheet.CreateRow (rowIndex + 1)
        |> fun row ->
            indexWithTitleAndFunc
            |> Array.iter (fun (columnIndex, _, func) ->
                row.CreateCell columnIndex
                |> fun cell -> cell.SetCellValue (func item)))

    let stream = new MemoryStream ()
    book.Write stream
    stream.Position <- int64 0
    stream
