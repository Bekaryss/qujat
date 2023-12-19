import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const getCurrentCategory = createAsyncThunk(
  "/category/fetchGetCurrentCategory",
  async (id) => {
    try {
      const response = await instance.get(`/api/1/categories/${id}`);
      if (response.status === 200) {
        return response.data.responseData;
      }
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
);

const initialState = {
  category: null,
  isLoading: true,
  error: "",
};

const currentCategory = createSlice({
  name: "category",
  initialState,
  extraReducers: (builder) => {
    builder
      .addCase(getCurrentCategory.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(getCurrentCategory.fulfilled, (state, action) => {
        state.isLoading = false;
        state.category = action.payload;
      })
      .addCase(getCurrentCategory.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

export default currentCategory.reducer;
