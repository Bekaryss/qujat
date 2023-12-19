import { configureStore } from "@reduxjs/toolkit";
import categoriesSlice from "./slices/categorySlice";
import tokenSlice from "./slices/tokenSlice";
import adminSlice from "./slices/adminSlice";
import actsSlice from "./slices/actsSlice";
import documentsSlice from "./slices/documentsSlice";
import subCategoriesSlice from "./slices/subCategoriesSlice";
import IconSlice from "./slices/IconSlice";
import currentCategory from "./slices/currentCategory";

export const store = configureStore({
  reducer: {
    token: tokenSlice,
    adminInfo: adminSlice,
    categories: categoriesSlice,
    subCategories: subCategoriesSlice,
    acts: actsSlice,
    documents: documentsSlice,
    icons: IconSlice,
    category: currentCategory,
  },
});

export default store;
