import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchSubCategories = createAsyncThunk(
  "/subCategories/fetchSubCategories",
  async (categoryId) => {
    try {
      const response = await instance.get(
        `/api/1/categories/${categoryId}/subcategories`
      );

      if (response.status === 200) {
        // console.log(response.data.responseData)
        return response.data.responseData;
      }
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
);

export const saveReorderedSubCategories = createAsyncThunk(
  "/subCategories/saveReorderedSubCategories",
  async (subCategoriesObj) => {
    try {
      console.log(subCategoriesObj);
      const response = await instance.patch(
        `/api/1/categories/${subCategoriesObj.id}/subcategories/display-order`,
        JSON.stringify({
          subcategoryIdsByDisplayOrder: subCategoriesObj.data,
        })
      );

      if (response.status === 200) {
        alert("SubCategories saved");
      }
    } catch (error) {
      console.log(error);
    }
  }
);

const initialState = {
  subCategories: [],
  isLoading: true,
  error: "Error",
};

const subCategoriesSlice = createSlice({
  name: "subCategories",
  initialState,
  reducers: {
    reorderSubCategories: (state, action) => {
      state.subCategories = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchSubCategories.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(fetchSubCategories.fulfilled, (state, action) => {
        state.isLoading = false;
        state.subCategories = action.payload;
      })
      .addCase(fetchSubCategories.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

// экспортируем экшены, если нужно
export const { reorderSubCategories } = subCategoriesSlice.actions;

export default subCategoriesSlice.reducer;
