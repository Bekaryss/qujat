import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchLegalGrounds = createAsyncThunk(
    "legalGrounds/fetchLegalGrounds",
    async () => {
        try {
            const response = await instance.get(`/api/1/frontend/links-page/links?pageIndex=0&pageSize=10`);
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
    legalGrounds: [],
    isLoading: false,
    error: "Error",
};

const legalGroundsSlice = createSlice({
    name: "legalGrounds",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchLegalGrounds.pending, (state, action) => {
                state.isLoading = true;
            })
            .addCase(fetchLegalGrounds.fulfilled, (state, action) => {
                state.isLoading = false;
                state.legalGrounds = action.payload;
            })
            .addCase(fetchLegalGrounds.rejected, (state, action) => {
                state.error = action.error.message;
            });
    },
});

export default legalGroundsSlice.reducer;
