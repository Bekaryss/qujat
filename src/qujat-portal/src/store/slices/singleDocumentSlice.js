import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchSingleDocument = createAsyncThunk(
    "singleDocument/fetchSingleDocument",
    async (data) => {
        try {
            const response = await instance.get(`/api/1/frontend/single-document-page/category/${data.categoryId}/subcategories/${data.subCategoryId}/document/${data.documentId}`);
            if (response.status === 200) {
                console.log(response)
                return response.data.responseData
            }
        } catch (error) {
            console.error(error);
            throw error;
        }
    }
);

const initialState = {
    singleDocument: [],
    isLoading: false,
    error: "Error",
};

const singleDocumentSlice = createSlice({
    name: "singleDocument",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchSingleDocument.pending, (state, action) => {
                state.isLoading = true;
            })
            .addCase(fetchSingleDocument.fulfilled, (state, action) => {
                state.isLoading = false;
                state.singleDocument = action.payload;
            })
            .addCase(fetchSingleDocument.rejected, (state, action) => {
                state.error = action.error.message;
            });
    },
});

export default singleDocumentSlice.reducer;
