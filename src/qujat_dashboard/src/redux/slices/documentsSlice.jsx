import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../axios/axios";

export const fetchDocuments = createAsyncThunk(
  "/fetch/documetns",
  async ({ pageIndex, search, sortProperty, sortOrder }) => {
    try {
      console.log(search);
      let uri = `/api/1/documents?pageIndex=${pageIndex}&pageSize=10`;
      if (search !== "") {
        uri += `&nameSearchPattern=${search}`;
      }
      let sort = `&sortProperty=${sortProperty}&sortOrder=${sortOrder}`;
      const response = await instance.get(uri + sort);

      if (response.status === 200) {
        return response.data;
      }
    } catch (error) {
      console.error("Произошла ошибка:", error);
    }
  }
);

// export const addBlob = createAsyncThunk(
//   "category/fetchCategory",
//   async (data) => {
//     try {
//       const response = await instance.post("/api/1/blobs", data, {
//         headers: {
//           "Content-Type": "multipart/form-data",
//         },
//       });

//       if (response.status === 200) {
//         return response.data;
//       }
//     } catch (error) {
//       console.error("Произошла ошибка:", error);
//     }
//   }
// );

const initialState = {
  documents: [],
  pageIndex: 0,
  documentId: 0,
  isDocumentAdded: false,
  isLoading: true,
  error: "Error",
};

const documentSlice = createSlice({
  name: "documents",
  initialState,
  reducers: {
    changePageIndex: (state, action) => {
      state.pageIndex = action.payload;
    },
    setDocumentId: (state, action) => {
      state.documentId = action.payload;
    },
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
      });
  },
});

export const { changePageIndex, setDocumentId } = documentSlice.actions;

export default documentSlice.reducer;
