import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchIcons = createAsyncThunk("/api/1/icons", async () => {
  try {
    const response = await instance.get("/api/1/icons");

    if (response.status === 200) {
      return response.data.responseData;
    }

    // if (response.status === 400) {
    //   const responseData = await response.json();
    //   console.log(responseData);
    // }
  } catch (error) {
    console.error(error);
    throw error;
  }
});

const initialState = {
  icons: null,
  isLoading: true,
  error: "",
};

const IconSlice = createSlice({
  name: "icons",
  initialState,

  extraReducers: (builder) => {
    builder
      .addCase(fetchIcons.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(fetchIcons.fulfilled, (state, action) => {
        state.isLoading = false;
        state.icons = action.payload;
      })
      .addCase(fetchIcons.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

export default IconSlice.reducer;
