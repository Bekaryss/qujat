import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchCategories = createAsyncThunk(
    "categories/fetchCategories",
    async (data) => {
        try {
            const response = await instance.get(`/api/1/frontend/main-page/categories?pageIndex=${data.pageIndex}&pageSize=10`);
            if (response.status === 200) {
                return response.data;
            }
        } catch (error) {
            console.error(error);
            throw error;
        }
    }
);

const initialState = {
    categories: [],
    totalCategories: 0,
    currentCategory: {},
    isLoading: false,
    error: "Error",
};

const categoriesSlice = createSlice({
    name: "categories",
    initialState,
    reducers: {
        setCurrentCategory: (state, action) => {
            state.currentCategory = action.payload
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchCategories.pending, (state, action) => {
                state.isLoading = true;
            })
            .addCase(fetchCategories.fulfilled, (state, action) => {
                state.isLoading = false;
                state.categories = action.payload.responseData;
                state.totalCategories = action.payload.totalSize
            })
            .addCase(fetchCategories.rejected, (state, action) => {
                state.error = action.error.message;
            });
    },
});

export const { setCurrentCategory } = categoriesSlice.actions

export default categoriesSlice.reducer;
