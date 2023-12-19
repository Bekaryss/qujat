import { configureStore } from "@reduxjs/toolkit"
import categoriesSlice from "./slices/categoriesSlice.js"
import subCategoriesSlice from "./slices/subCategoriesSlice.js"
import documentsSlice from './slices/documentsSlice.js'
import legalGroundsSlice from './slices/legalGroundsSlice.js'
import SingleDocumentSlice from './slices/singleDocumentSlice.js'

const store = configureStore({
    reducer: {
        categories: categoriesSlice,
        subCategories: subCategoriesSlice,
        documents: documentsSlice,
        singleDocument: SingleDocumentSlice,
        legalGrounds: legalGroundsSlice
    }
})

export default store