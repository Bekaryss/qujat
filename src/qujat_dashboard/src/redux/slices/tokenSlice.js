import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const loginFetch = createAsyncThunk("/login/fetchUser", async (data) => {
  try {
    const response = await instance.post("/api/1/identity/sign-in", data);

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

export const AuthMe = createAsyncThunk("/login/authMe", async (data) => {
  try {
    const response = await instance.get("/api/1/identity/me", data);

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
  token: null,
  isLoading: true,
  error: "",
};

const userAuth = createSlice({
  name: "user",
  initialState,
  reducers: {
    logOut: (state) => {
      state.token = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loginFetch.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(loginFetch.fulfilled, (state, action) => {
        state.isLoading = false;
        state.token = action.payload;
      })
      .addCase(loginFetch.rejected, (state, action) => {
        state.error = action.error.message;
      })
      .addCase(AuthMe.pending, (state, action) => {
        state.isLoading = true;
      })
      .addCase(AuthMe.fulfilled, (state, action) => {
        state.isLoading = false;
        state.token = action.payload;
      })
      .addCase(AuthMe.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

export const selectIsAuth = (state) => Boolean(state.token.token);
export const { logOut } = userAuth.actions;
export default userAuth.reducer;
