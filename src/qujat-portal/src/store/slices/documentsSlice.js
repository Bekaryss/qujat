import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchDocuments = createAsyncThunk(
    "documents/fetchDocuments",
    async (data) => {
        try {
            const response = await instance.get(`/api/1/frontend/subcategories-page/${data.categoryId}/subcategories/${data.subCategoryId}/documents?pageIndex=0&pageSize=10`);
            if (response.status === 200) {
                return response.data.responseData;
            }
        } catch (error) {
            console.error(error);
            throw error;
        }
    }
);

export const fetchTopDocuments = createAsyncThunk(
    "topDocuments/fetchTopDocuments",
    async (data) => {
        try {
            const response = await instance.get('/api/1/frontend/main-page/documents/top');
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
    documents: [],
    currentDocument: {},
    topDocuments: [],
    isLoading: false,
    error: "Error",
};

const documentsSlice = createSlice({
    name: "documents",
    initialState,
    reducers: {
        setCurrentDocument: (state, action) => {
            state.currentDocument = action.payload
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchDocuments.pending, (state, action) => {
                state.isLoading = true;
            })
            .addCase(fetchDocuments.fulfilled, (state, action) => {
                state.isLoading = false;
                state.documents = action.payload;
            })
            .addCase(fetchDocuments.rejected, (state, action) => {
                state.error = action.error.message;
            })
            .addCase(fetchTopDocuments.pending, (state, action) => {
                    state.isLoading = true;
                })
            .addCase(fetchTopDocuments.fulfilled, (state, action) => {
                state.isLoading = false;
                state.topDocuments = action.payload;
            })
            .addCase(fetchTopDocuments.rejected, (state, action) => {
                state.error = action.error.message;
            });
    },
});

export const { setCurrentDocument } = documentsSlice.actions

export default documentsSlice.reducer;
