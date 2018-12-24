const fs = require('fs')
async function readFileStream ({ filePath, minChunkLength, onStringFunc }) {
   minChunkLength = parseInt(minChunkLength) || 1024 * 1024
   return new Promise((resolve, reject) => {
     let preLeaveString = ''
     fs.createReadStream(filePath)
       .on('data', data => {
         const newString = (data && data.toString('utf8')) || ''
	         const currentString = preLeaveString + newString
         const lastIndexOfReturn = currentString.lastIndexOf('\n')
         if (lastIndexOfReturn > minChunkLength) {
           const flushString = currentString.substring(0, lastIndexOfReturn)
           onStringFunc && onStringFunc(flushString)
           preLeaveString = currentString.substring(lastIndexOfReturn + 1)
         } else {
           preLeaveString = currentString
         }
       })
       .on('end', () => {
         if (preLeaveString.length > 0) {
           onStringFunc && onStringFunc(preLeaveString)
         }
         resolve()
       })
       .on('error', error => {
         const errorInstance = error instanceof Error ? error : new Error(error)
         reject(errorInstance)
       })
   })
 }
