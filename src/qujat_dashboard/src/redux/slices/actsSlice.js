import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchActs = createAsyncThunk(
  "fetch/ActsFetch",
  async (pageIndex) => {
    try {
      const response = await instance.get(
        `/api/1/links?pageIndex=${pageIndex}&pageSize=10`
      );

      if (response.status === 200) {
        return response.data;
      }
    } catch (error) {
      console.log(error);
    }
  }
);

export const deleteAct = createAsyncThunk("delete/Act", async (id) => {
  try {
    const response = await instance.delete(`/api/1/links/${id}`, {
      body: JSON.stringify({}),
    });
    console.log(id);
    if (response.status === 200) {
      return response.data.responseData;
    }
  } catch (error) {
    console.log(error);
  }
});

const initialState = {
  acts: null,
  id: 0,
  pageIndex: 0,
  isLoading: true,
  error: "",
};

const actsSlice = createSlice({
  name: "acts",
  initialState,
  reducers: {
    setId: (state, action) => {
      state.id = action.payload;
    },
    changePageIndex: (state, action) => {
      state.pageIndex = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchActs.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(fetchActs.fulfilled, (state, action) => {
        state.isLoading = false;
        state.acts = action.payload;
      })
      .addCase(fetchActs.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

export const { setId, changePageIndex } = actsSlice.actions;

export default actsSlice.reducer;
