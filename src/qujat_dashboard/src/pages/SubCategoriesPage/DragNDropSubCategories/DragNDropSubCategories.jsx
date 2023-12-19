import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { Box, Button } from "@mui/material";

import {
  fetchSubCategories,
  reorderSubCategories,
  saveReorderedSubCategories,
} from "../../../redux/slices/subCategoriesSlice.js";

import { DragDropContext, Draggable, Droppable } from "react-beautiful-dnd";
import { MdEdit } from "react-icons/md";
import { MdOutlineDeleteSweep } from "react-icons/md";

import AddSubCategoryForm from "../../../components/Modals/AddSubCategoryForm/AddSubcCategoryForm.jsx";
import DeleteSubCategory from "../../../components/Modals/DeleteSubCategory/DeleteSubCategory.jsx";
import UpdateSubCategory from "../../../components/Modals/UpdateSubCategory/UpdateSubCategory.jsx";
import instance from "../../../axios/axios.js";

const DragNDropSubCategories = () => {
  const [openAddSubCategories, setOpenAddSubCategories] = useState(false);
  const [openDelSubCategories, setOpenDelSubCategories] = useState(false);
  const [openUpdateSubCategories, setOpenUpdateSubCategories] = useState(false);
  const [id, setId] = useState(0);

  const { categoryId } = useParams();
  const { subCategories, isLoading } = useSelector(
    (state) => state.subCategories
  );
  const [dataLength, setDataLength] = useState(subCategories.length);
  const dispatch = useDispatch();

  const handleOpenModal = () => {
    setOpenAddSubCategories(true);
  };
  const handleCloseModal = () => {
    setOpenAddSubCategories(false);
  };

  const handleDeleteOpen = (id) => {
    setOpenDelSubCategories(true);
    setId(id);
  };
  const handleDeleteClose = () => {
    setOpenDelSubCategories(false);
  };

  const handleUpdateOpen = (id) => {
    setOpenUpdateSubCategories(true);
    setId(id);
  };

  const handleUpdateClose = () => {
    setOpenUpdateSubCategories(false);
  };

  const onDragEnd = (result) => {
    if (!result.destination) return;

    const updatedSubCategories = Array.from(subCategories);
    const [reorderedItem] = updatedSubCategories.splice(result.source.index, 1);
    updatedSubCategories.splice(result.destination.index, 0, reorderedItem);

    // Update the displayOrder based on the new order
    const updatedSubCategoriesWithDisplayOrder = updatedSubCategories.map(
      (category, index) => ({
        ...category,
        displayOrder: index,
      })
    );
    dispatch(reorderSubCategories(updatedSubCategoriesWithDisplayOrder));
    console.log(updatedSubCategoriesWithDisplayOrder);
  };

  const handleReorderSave = () => {
    dispatch(
      saveReorderedSubCategories({
        id: categoryId,
        data: subCategories.map((el) => el.id),
      })
    );
  };

  useEffect(() => {
    if (categoryId !== undefined) {
      dispatch(fetchSubCategories(categoryId));
    }
  }, [categoryId, dispatch, dataLength, id]);

  return (
    <>
      {!isLoading && (
        <div style={{ width: "100%", overflowY: "auto" }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              marginTop: 2,
              marginBottom: 2,
            }}
          >
            <Button
              sx={{
                background: "var(--main-color-500)",
                color: "#fff",
                "&:hover": {
                  backgroundColor: "var(--main-color-btn)",
                },
              }}
              variant="outlined"
              onClick={handleReorderSave}
            >
              САҚТАУ
            </Button>
            <Button
              variant="outlined"
              sx={{
                background: "var(--main-color-500)",
                color: "#fff",
                "&:hover": {
                  backgroundColor: "var(--main-color-btn)",
                },
              }}
              onClick={handleOpenModal}
            >
              AAAAAAAAAAA ҚОСУ
            </Button>
          </Box>
          <DragDropContext onDragEnd={onDragEnd}>
            <Droppable droppableId="tasks" direction="vertical">
              {(provided) => (
                <Box
                  sx={{
                    display: "flex",
                    width: "100%",
                    flexWrap: "wrap",
                    gap: "10px",
                  }}
                  {...provided.droppableProps}
                  ref={provided.innerRef}
                >
                  {subCategories.map((item, index) => (
                    <Draggable
                      key={item.displayOrder}
                      draggableId={item.displayOrder.toString()}
                      index={index}
                    >
                      {(provided) => (
                        <Box
                          ref={provided.innerRef}
                          {...provided.draggableProps}
                          {...provided.dragHandleProps}
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            height: "80px",
                            width: "100%",
                            background: "#fff",
                            padding: "24px",
                            borderRadius: "10px",
                          }}
                        >
                          <h2>{item.nameKz}</h2>
                          <Box sx={{ display: "flex", alignItems: "center" }}>
                            <Box
                              sx={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                border: "1px solid var(--black)",
                                borderRadius: "8px",
                                padding: 0.5,
                                cursor: "pointer",
                                marginRight: 2,
                                transition: "var(--tran-bg-03)",
                                "&:hover": {
                                  background: "var(--main-color-btn)",
                                },
                              }}
                              onClick={() => handleUpdateOpen(item.id)}
                            >
                              <MdEdit style={{ fontSize: "20px" }} />
                            </Box>
                            <Box
                              sx={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                border: "1px solid var(--black)",
                                borderRadius: "8px",
                                padding: 0.5,
                                cursor: "pointer",
                                transition: "var(--tran-bg-03)",
                                "&:hover": {
                                  background: "var(--main-color-btn)",
                                },
                              }}
                              onClick={() => handleDeleteOpen(item.id)}
                            >
                              <MdOutlineDeleteSweep
                                style={{
                                  fontSize: "20px",
                                  color: "var(--main-red)",
                                }}
                              />
                            </Box>
                          </Box>
                        </Box>
                      )}
                    </Draggable>
                  ))}
                  {provided.placeholder}
                </Box>
              )}
            </Droppable>
          </DragDropContext>
          <AddSubCategoryForm
            isOpenModal={openAddSubCategories}
            handleClose={handleCloseModal}
            setDataLength={setDataLength}
          />
          <DeleteSubCategory
            isOpenDelete={openDelSubCategories}
            handleCloseDelete={handleDeleteClose}
            subCategoryId={id}
            setDataLength={setDataLength}
          />
          <UpdateSubCategory
            isOpenUpdate={openUpdateSubCategories}
            handleCloseUpdate={handleUpdateClose}
            subCategoryId={id}
          />
        </div>
      )}
    </>
  );
};

export default DragNDropSubCategories;
