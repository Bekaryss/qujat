import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchSubCategories = createAsyncThunk(
    "SubCategories/fetchSubCategories",
    async (categoryId) => {
        try {
            const response = await instance.get(`https://qujat-temp-front-api.zonakomforta.kz/api/1/frontend/subcategories-page/${categoryId}/subcategories?pageIndex=0&pageSize=10`);

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
    subCategories: [],
    currentSubCategory: {},
    isLoading: false,
    error: "Error",
};

const subCategoriesSlice = createSlice({
    name: "subCategories",
    initialState,
    reducers: {
        setCurrentSubCategory: (state, action) => {
            state.currentSubCategory = action.payload
        }
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

export const { setCurrentSubCategory } = subCategoriesSlice.actions

export default subCategoriesSlice.reducer;