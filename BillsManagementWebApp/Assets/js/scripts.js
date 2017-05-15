function FormatJSDateToCSharpDate(objDate) {    
    return [objDate.getFullYear(), objDate.getMonth() + 1 <= 9 ? '0' + (objDate.getMonth() + 1) : objDate.getMonth() + 1, objDate.getDate() <= 9 ? '0' + objDate.getDate() : objDate.getDate()].join('-');
}

function FormatJSDateToString(objDate) {
    return [objDate.getFullYear(), objDate.getMonth() + 1 <= 9 ? '0' + (objDate.getMonth() + 1) : objDate.getMonth() + 1, objDate.getDate() <= 9 ? '0' + objDate.getDate() : objDate.getDate()].reverse().join('.');
}