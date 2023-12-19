import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchCategory = createAsyncThunk(
  "/categories/fetchCategory",
  async (page = 0) => {
    try {
      const response = await instance.get(
        `api/1/categories?pageIndex=${page}&pageSize=0`
      );

      if (response.status === 200) {
        return response.data.responseData;
      }
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
);

export const addNewCategory = createAsyncThunk(
  "/categories/fetchNewCategory",
  async (data) => {
    try {
      const response = await instance.post("/api/1/categories", data);

      if (response.status === 200) {
        return response.data.responseData;
      }
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
);

export const deleteCategory = createAsyncThunk(
  "/categories/fetchDeleteCategory",
  async (id) => {
    try {
      const response = await instance.delete(`/api/1/categories/${id}`, {
        body: JSON.stringify({}),
      });

      if (response.status === 200) {
        return response.data;
      }
    } catch (error) {
      throw error;
    }
  }
);

export const saveReorderedCategories = createAsyncThunk(
  "/categories/saveReorderedCategories",
  async (data) => {
    try {
      const response = await instance.patch(
        "/api/1/categories/display-order",
        JSON.stringify({
          categoryIdsByDisplayOrder: data,
        })
      );

      if (response.status === 200) {
        return response.data;
      }
    } catch (error) {
      console.log(error);
      throw error;
    }
  }
);

const initialState = {
  categories: [],
  displayOrder: [],
  reorderedCategoriesStatus: null,
  isLoading: true,
  error: "",
};

const categoriesSlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    reorderCategories: (state, action) => {
      state.displayOrder = action.payload.map((item) => item.id);
    },
    setIsLoading: (state, action) => {
      state.isLoading = action.payload;
      console.log(state.isLoading);
    },
    setCategories: (state, action) => {
      state.categories = action.payload;
    },
    setError: (state) => {
      state.error = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCategory.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(fetchCategory.fulfilled, (state, action) => {
        state.isLoading = false;
        state.categories = action.payload;
      })
      .addCase(fetchCategory.rejected, (state, action) => {
        state.error = action.error.message;
      })
      .addCase(deleteCategory.rejected, (state, action) => {
        // Handle the error here
        state.error = action.error.message;
      })
      .addCase(saveReorderedCategories.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(saveReorderedCategories.fulfilled, (state, action) => {
        state.isLoading = false;
        state.reorderedCategoriesStatus = action.payload;
      })
      .addCase(saveReorderedCategories.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

// экспортируем экшены, если нужно
export const { reorderCategories, setIsLoading, setCategories, setError } =
  categoriesSlice.actions;

export default categoriesSlice.reducer;
