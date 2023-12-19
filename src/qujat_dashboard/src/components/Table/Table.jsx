import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Button,
  styled,
  IconButton,
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableBody,
  TableCell,
  Collapse,
  Box,
  Pagination,
  useMediaQuery,
  useTheme,
} from "@mui/material";

import { IoIosArrowUp, IoIosArrowDown } from "react-icons/io";

import edit from "../../assets/images/edit.svg";
import trash from "../../assets/images/trash.svg";

import AddCateoryForm from "../Modals/AddCategoryForm/AddCateoryForm";
import Update from "../Modals/UpdateCategory/Update.jsx";
import Delete from "../Modals/Delete/Delete.jsx";

import { fetchCategory } from "../../redux/slices/categorySlice.js";

import "./Table.css";

const AddButton = styled(Button)({
  backgroundColor: "#16aaff",
  color: "#fff",
  padding: 11,
  "&:hover": {
    backgroundColor: "#0486d1",
  },
  "@media (max-width: 475px)": {
    width: "100%",
    marginTop: 8,
  },
});

// const isLoadingCategory = state.category === true;

const TableCategory = () => {
  const theme = useTheme();
  const isSmallScreen = useMediaQuery(theme.breakpoints.down("sm"));
  const dispatch = useDispatch();

  const [modalOpen, setModalOpen] = useState(false);
  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [deleteItem, setDeleteItem] = useState(false);
  const [selectedRow, setSelectedRow] = useState(null);
  const [id, setId] = useState(0);

  const { categories, isLoading } = useSelector((state) => state.categories);

  const [dataLength, setDataLength] = useState(categories.length);
  const isLoadingCategories = isLoading === true;

  const handleOpenModal = () => {
    setModalOpen(true);
  };
  const handleCloseModal = () => {
    setModalOpen(false);
  };

  // UPDATE MODAL HANDLER
  const handleUpdateOpenModal = (id) => {
    setUpdateModalOpen(true);
    setId(id);
  };
  const handleUpdateCloseModal = () => {
    setUpdateModalOpen(false);
  };

  // DELETE MODAL HANDLER
  const handleOpenDeleteModal = (id) => {
    setDeleteItem(true);
    setId(id);
  };

  const handleCloseDeleteModal = () => {
    setDeleteItem(false);
    setDataLength((prev) => prev + 1);
  };

  useEffect(() => {
    dispatch(fetchCategory());
  }, [dispatch, dataLength]);

  return (
    <div className="table">
      <div className="table_head">
        <AddButton onClick={handleOpenModal}>
          <span className="btn_text">қосу</span>
        </AddButton>
      </div>
      <AddCateoryForm isOpen={modalOpen} handleClose={handleCloseModal} />
      <TableContainer component={Paper}>
        <Table aria-label="simple table">
          <TableHead className="table_content_head">
            <TableRow sx={{ alignItems: "center" }}>
              <TableCell></TableCell>
              <TableCell>
                <h2>#</h2>
              </TableCell>
              <TableCell>
                <h2>Тақырыптық санат аты</h2>
              </TableCell>
              <TableCell>
                <h2>Иконка</h2>
              </TableCell>
              <TableCell>
                <h2>Әрекет</h2>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {!isLoadingCategories &&
              categories?.map((item) => (
                <>
                  <TableRow
                    key={item.id}
                    sx={{
                      cursor: "pointer",
                    }}
                    className="table_item"
                  >
                    <TableCell>
                      {/* {row.subCategory.length > 0 && ( */}
                      <IconButton
                        aria-label="expand row"
                        size="small"
                        onClick={() =>
                          setSelectedRow(
                            selectedRow === item.id ? null : item.id
                          )
                        }
                      >
                        {selectedRow === item.id ? (
                          <IoIosArrowUp className="arrow_icon_size" />
                        ) : (
                          <IoIosArrowDown className="arrow_icon_size" />
                        )}
                      </IconButton>
                      {/* )} */}
                    </TableCell>
                    <TableCell>{item.id}</TableCell>
                    <TableCell>{item.nameKz}</TableCell>
                    <TableCell>
                      {item.iconBlob?.uri ? (
                        <img src={item.iconBlob?.uri} className="icon_size" />
                      ) : (
                        <span>ҚУЫС</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <img
                        src={edit}
                        onClick={() => {
                          handleUpdateOpenModal(item.id);
                        }}
                        style={{ marginRight: "8px" }}
                      />
                      <img
                        src={trash}
                        onClick={() => handleOpenDeleteModal(item.id)}
                      />
                    </TableCell>
                  </TableRow>
                  {/* {row.subCategory.length > 0 && (
                  <TableRow>
                    <TableCell colSpan={6}>
                      <Collapse
                        in={selectedRow === row.id}
                        timeout="auto"
                        unmountOnExit
                      >
                        <TableSubCategory data={row?.subCategory} />
                      </Collapse>
                    </TableCell>
                  </TableRow>
                )} */}
                </>
              ))}
          </TableBody>
        </Table>

        {/* PAGINATION */}
        <Box
          spacing={2}
          sx={{
            width: "100%",
            display: "flex",
            alignContent: "center",
            justifyContent: "center",
            padding: 3,
          }}
        >
          <Pagination
            size={isSmallScreen ? "small" : "medium"}
            count={10}
            variant="outlined"
            shape="rounded"
          />
        </Box>
      </TableContainer>

      <Update
        updateModalOpen={updateModalOpen}
        handleUpdateCloseModal={handleUpdateCloseModal}
      />

      <Delete
        deleteItem={deleteItem}
        handleCloseDeleteModal={handleCloseDeleteModal}
        id={id}
      />
    </div>
  );
};

export default TableCategory;

const data = [
  {
    id: 1,
    name: "test",
    date: "01.01.01",
    test: "test",
    subCategory: [
      { id: 1231, name: "test", date: "01.01.01" },
      { id: 1232, name: "test", date: "01.01.01" },
      { id: 1233, name: "test", date: "01.01.01" },
    ],
  },
  {
    id: 2,
    name: "test",
    date: "01.01.01",
    test: "test",
    subCategory: [{ id: 222, name: "test", date: "01.01.01" }],
  },
  {
    id: 3,
    name: "test",
    date: "01.01.01",
    test: "test",
    subCategory: [],
  },
];
